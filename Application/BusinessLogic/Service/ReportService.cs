using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Domain;

namespace Application.BusinessLogic.Service;

public class ReportService : IReportService
{
    private readonly IDiagnosisRepository _diagnosisRepository;
    private readonly IIcdRepository _icdRepository;
    private readonly IPatientMapper _patientMapper;
    private readonly IReportMapper _reportMapper;
    
    public ReportService(
        IDiagnosisRepository diagnosisRepository,
        IIcdRepository icdRepository,
        IPatientMapper patientMapper,
        IReportMapper reportMapper)
    {
        _diagnosisRepository = diagnosisRepository;
        _icdRepository = icdRepository;
        _patientMapper = patientMapper;
        _reportMapper = reportMapper;
    }

    public async Task<IcdRootsReportDto> GetReport(DateTime start, DateTime end, List<Guid> icdRoots)
    {
        var diagnoses = await _diagnosisRepository.GetAllDiagnoses();
        if (icdRoots.Any())
        {
            diagnoses = await FilterDiagnosisByIcdRoots(diagnoses, icdRoots);
        }
        else
        {
            foreach (var diagnosis in diagnoses)
            {
                var icdRoot = await _icdRepository.GetRootByIcdId(diagnosis.icd.id);
                icdRoots.Add(icdRoot.id);
            }

            icdRoots = icdRoots.Distinct().ToList();
        }

        var inspectionsInInterval = diagnoses
            .Select(d => d.inspection)
            .Where(i => i.date <= end && i.date >= start)
            .ToList();

        var patients = inspectionsInInterval.Select(i => i.patient).Distinct().ToList();

        var totalVisitsByRoot = icdRoots.ToDictionary(root => root, root => 0);
        var reportData = new List<IcdRootsReportRecordDto>();

        foreach (var patient in patients)
        {
            var visitsByRoot = icdRoots.ToDictionary(root => root, root => 0);

            var patientInspections = inspectionsInInterval
                .Where(i => i.patient.id == patient.id)
                .ToList();

            foreach (var inspection in patientInspections)
            {
                var inspectionDiagnoses = diagnoses
                    .Where(d => d.inspection.id == inspection.id)
                    .ToList();

                foreach (var diagnosis in inspectionDiagnoses)
                {
                    var diagnosisIcdRoot = await _icdRepository.GetRootByIcdId(diagnosis.icd.id);
                    if (visitsByRoot.ContainsKey(diagnosisIcdRoot.id))
                    {
                        visitsByRoot[diagnosisIcdRoot.id]++;
                        totalVisitsByRoot[diagnosisIcdRoot.id]++;
                    }
                }
            }

            var reportRecord = _patientMapper.ToIcdRootsReportRecordDto(patient, visitsByRoot);
            reportData.Add(reportRecord);
        }

        reportData = reportData.OrderBy(r => r.patientName).ToList();

        var report = _reportMapper.ToDto(start, end, icdRoots, reportData, totalVisitsByRoot);

        return report;
    }
    
    private async Task<List<Diagnosis>> FilterDiagnosisByIcdRoots(List<Diagnosis> diagnoses, List<Guid> icdRoots)
    {
        var filteredDiagnoses = new List<Diagnosis>();
        foreach (var diagnosis in diagnoses)
        {
            bool hasRoot = false;

            foreach (var root in icdRoots)
            {
                var diagnosisIcdRoot = await _icdRepository.GetRootByIcdId(diagnosis.icd.id);
                if (diagnosisIcdRoot.id == root)
                {
                    hasRoot = true;
                    break; 
                }
            }

            if (hasRoot)
            {
                filteredDiagnoses.Add(diagnosis);
            }
        }

        return filteredDiagnoses;
    }
}