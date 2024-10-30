using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Domain;

namespace Application.BusinessLogic.Service;

public class ReportService : IReportService
{
    private readonly IInspectionRepository _inspectionRepository;
    private readonly IDiagnosisRepository _diagnosisRepository;
    private readonly IIcdRepository _icdRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientMapper _patientMapper;
    private readonly IReportMapper _reportMapper;
    
    public ReportService(
        IInspectionRepository inspectionRepository,
        IDiagnosisRepository diagnosisRepository,
        IIcdRepository icdRepository,
        IPatientRepository patientRepository,
        IPatientMapper patientMapper,
        IReportMapper reportMapper)
    {
        _inspectionRepository = inspectionRepository;
        _diagnosisRepository = diagnosisRepository;
        _icdRepository = icdRepository;
        _patientRepository = patientRepository;
        _patientMapper = patientMapper;
        _reportMapper = reportMapper;
    }

    public async Task<IcdRootsReportDto> GetReport(DateTime start, DateTime end, List<Guid> icdRoots)
    {
        var diagnoses = await _diagnosisRepository.GetAllDiagnoses();
        if (icdRoots.Any())
        {
            var filteredDiagnoses = new List<Diagnosis>();

            foreach (var diagnosis in diagnoses)
            {
                bool hasRoot = false;

                foreach (var root in icdRoots)
                {
                    if (await IsHasIcdRoot(diagnosis.icd, root))
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

            diagnoses = filteredDiagnoses;
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
            .Distinct()
            .ToList();
        
        var patients = await _patientRepository.GetAllPatients();
        
        var totalVisitsByRoot = new Dictionary<Guid, int>();
        var reportData = new List<IcdRootsReportRecordDto>();

        foreach (var patient in patients)
        {
            var visitsByRoot = new Dictionary<Guid, int>();

            foreach (var root in icdRoots)
            {
                int visitCount = 0;

                foreach (var inspection in inspectionsInInterval)
                {
                    if (inspection.patient.id == patient.id)
                    {
                        foreach (var diagnosis in diagnoses)
                        {
                            if (diagnosis.inspection.id == inspection.id && await IsHasIcdRoot(diagnosis.icd, root))
                            {
                                visitCount++; 
                                break;
                            }
                        }
                    }
                }

                visitsByRoot[root] = visitCount; 
                totalVisitsByRoot.TryGetValue(root, out var currentCount);
                totalVisitsByRoot[root] = currentCount + visitCount;
            }
            
            var reportRecord = _patientMapper.ToIcdRootsReportRecordDto(patient, visitsByRoot);
            reportData.Add(reportRecord);
        }
        
        reportData = reportData.OrderBy(r => r.patientName).ToList();
        
        var report = _reportMapper.ToDto(start, end, icdRoots, reportData, totalVisitsByRoot);

        return report;
        
        

    }
    
    private async Task<bool> IsHasIcdRoot(Icd icd, Guid rootId)
    {
        var currentIcd = icd;

        while (currentIcd != null)
        {
            if (icd.id == rootId)
            {
                return true;
            }

            if (icd.parent == null)
            {
                return false;
            }

            currentIcd = await _icdRepository.GetById(currentIcd.parent.id);
        }

        return false;
    }
    
    
}