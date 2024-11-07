using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;
using Domain;
using Domain.Enums;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class ReportService(
    IDiagnosisService diagnosisService,
    IIcdRepository icdRepository,
    IPatientMapper patientMapper,
    IReportMapper reportMapper,
    IDiagnosisRepository diagnosisRepository)
    : IReportService
{
    public async Task<IcdRootsReportDto> GetReport(DateTime start, DateTime end, List<Guid> icdRoots)
    {
        if (start > end)
        {
            throw new ValidationException("Start must be less or equals than end");
        }

        var diagnoses = await diagnosisRepository.GetAllMainDiagnoses();
        if (icdRoots.Any())
        {
            foreach (var root in icdRoots)
            {
                var icd = await icdRepository.GetById(root);
                if (icd == null)
                {
                    throw new IcdNotFoundException();
                }
                if (icd.parent != null)
                {
                    throw new IcdNotRootException();
                }
            }
            diagnoses = await diagnosisService.FilterDiagnosisByIcdRoots(diagnoses, icdRoots);
        }
        else
        {
            foreach (var diagnosis in diagnoses)
            {
                var icdRoot = await icdRepository.GetRootByIcdId(diagnosis.icd.id);
                icdRoots.Add(icdRoot.id);
            }

            icdRoots = icdRoots.Distinct().ToList();
        }

        var inspectionsInInterval = diagnoses
            .Select(d => d.inspection)
            .Where(i => i.date <= end && i.date >= start)
            .ToList();

        var patients = inspectionsInInterval.Select(i => i.patient).Distinct().ToList();
        patients = patients.OrderBy(p => p.name).ToList();

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
                var inspectionMainDiagnosis = diagnoses
                    .First(d => 
                        d.inspection.id == inspection.id && 
                        d.diagnosisType == DiagnosisType.Main);
                
                var diagnosisIcdRoot = await icdRepository.GetRootByIcdId(inspectionMainDiagnosis.icd.id);
                if (visitsByRoot.ContainsKey(diagnosisIcdRoot.id))
                {
                    visitsByRoot[diagnosisIcdRoot.id]++;
                    totalVisitsByRoot[diagnosisIcdRoot.id]++;
                }
            }

            var reportRecord = patientMapper.ToIcdRootsReportRecordDto(patient, visitsByRoot);
            reportData.Add(reportRecord);
        }

        reportData = reportData.OrderBy(r => r.patientName).ToList();

        var report = reportMapper.ToDto(start, end, icdRoots, reportData, totalVisitsByRoot);

        return report;
    }
}