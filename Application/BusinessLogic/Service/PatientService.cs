using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.BusinessLogic.Enums;
using Application.Dto;
using Application.Exceptions;
using Domain;
using Domain.Enums;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class PatientService(
    IPatientRepository patientRepository,
    IPatientMapper patientMapper,
    IValidator<PatientCreateRequest> patientCreateValidator,
    IIcdRepository icdRepository,
    IInspectionRepository inspectionRepository,
    IInspectionMapper inspectionMapper,
    IDiagnosisService diagnosisService,
    IDiagnosisRepository diagnosisRepository)
    : IPatientService
{
    public async Task<Guid> Create(PatientCreateRequest request)
    {
        var validation = await patientCreateValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }

        var patient = patientMapper.ToEntity(request);
       return await patientRepository.Create(patient);
    }

    public async Task<PatientDto> GetPatientById(Guid id)
    {
        var patient = await patientRepository.GetById(id);
        if (patient == null)
        {
            throw new PatientNotFoundException();
        }

        var patientDto = patientMapper.ToDto(patient);
        return patientDto;
    }
    
    public async Task<List<InspectionShortDto>> SearchPatientInspectionsByParams(Guid patientId, string? request)
    {
        var patient = await patientRepository.GetById(patientId);
        if (patient == null)
        {
            throw new PatientNotFoundException();
        }

        List<Inspection> inspections;
        if (request != null)
        {
            inspections = await inspectionRepository.GetRootInspectionsByPatient(patientId, request.ToLower());
        }
        else
        {
            inspections = await inspectionRepository.GetRootInspectionsByPatient(patientId);
        }
        
        List<InspectionShortDto> inspectionShortDtos = new List<InspectionShortDto>();
        foreach (var inspection in inspections)
        {
            var mainDiagnosisDto = await diagnosisService.GetMainDIagnosisByInspectionId(inspection.id);
            var inspectionShortDto = inspectionMapper.ToInspectionShortDto(inspection, mainDiagnosisDto);
            inspectionShortDtos.Add(inspectionShortDto);
        }

        return inspectionShortDtos;
    }

    public async Task<InspectionPagedListDto> GetPatientInspectionsByParams(
        Guid patientId,
        bool grouped,
        List<Guid> icdRoots,
        int page,
        int size)
    {
        if (page <= 0 || size <= 0)
        {
            throw new InvalidPaginationParamsException();
        }
        
        var patient = await patientRepository.GetById(patientId);
        if (patient == null)
        {
            throw new PatientNotFoundException();
        }

        var diagnoses = await diagnosisRepository.GetPatientsDiagnoses(patientId);
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
        
        var inspections = diagnoses.Select(d => d.inspection).Distinct().ToList();

        if (grouped)
        {
            inspections = inspections
                .Where(i => i.previousInspection == null)
                .ToList();
        }
        
        var overAllInspectionsCount = inspections.Count;
        var totalPages = (overAllInspectionsCount + size - 1) / size;
        
        if (page > totalPages)
        {
            throw new InvalidPaginationParamsException("Page must be smaller or equal page count.");
        }
        
        var pagedInspections = inspections
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
        
        List<InspectionFullDto> inspectionFullDtos = new List<InspectionFullDto>();
        foreach (var inspection in pagedInspections)
        {
            bool hasNested = await inspectionRepository.IsHasChild(inspection.id);
            bool hasChain = (hasNested || inspection.previousInspection != null);

            var mainDiagnosisDto = await diagnosisService.GetMainDIagnosisByInspectionId(inspection.id);
            var inspectionFullDto =
                inspectionMapper.ToInspectionFullDto(inspection, mainDiagnosisDto, hasChain, hasNested);
            inspectionFullDtos.Add(inspectionFullDto);
        }
        
        var pageInfo = new PageInfoDto(size, totalPages, page);

        return new InspectionPagedListDto(inspectionFullDtos, pageInfo);
    }

    public async Task<PatientPagedListDto> GetPatientsByParams(
        string? request,
        List<Conclusion>? conclusions,
        SortingType? sorting,
        bool scheduledVisits,
        bool onlyMine,
        int page, 
        int size,
        Guid doctorId)
    {
        if (page <= 0 || size <= 0)
        {
            throw new InvalidPaginationParamsException();
        }
        
        var patients = await patientRepository.GetAllPatients();

        if (request != null)
        {
            patients = patients.Where(patient => patient.name.ToLower().Contains(request.ToLower())).ToList();   
        }

        if (onlyMine)
        {
            patients = await FilterPatientsByDoctor(patients, doctorId);
        }

        if (scheduledVisits)
        {
            patients = await FilterBySheduledVisits(patients);
        }

        if (conclusions != null && conclusions.Any())
        {
            patients = await FilterPatientsByConclusion(patients, conclusions);
        }

        if (sorting != null)
        {
            var lastInspectionsDict = new Dictionary<Guid, DateTime?>();
            foreach (var patient in patients)
            {
                var inspections = await inspectionRepository.GetPatientInspections(patient.id); 
                var lastInspectionDate = inspections.Max(i => i.date); 
                lastInspectionsDict[patient.id] = lastInspectionDate;
            }
            patients = SortPatients(sorting.Value, patients, lastInspectionsDict);
        }
        
        var overAllPatientsCount = patients.Count;
        var totalPages = (overAllPatientsCount + size - 1) / size;
        
        if (page > totalPages)
        {
            throw new InvalidPaginationParamsException("Page must be smaller or equal page count.");
        }
        
        var pagedPatients = patients
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        List<PatientDto> patientDtos = pagedPatients
            .Select(p => patientMapper.ToDto(p))
            .ToList();
        var pageInfo = new PageInfoDto(size, totalPages, page);
        var patientPagedListDto = new PatientPagedListDto(patientDtos, pageInfo);

        return patientPagedListDto;
    }

    private List<Patient> SortPatients(
        SortingType type,
        List<Patient> patients, Dictionary<Guid, DateTime?> lastInspections)
    {
        switch (type)
        {
            case SortingType.CreateAsc:
                patients = patients.OrderBy(patient => patient.createTime).ToList();
                break;
                
            case SortingType.CreateDesc:
                patients = patients.OrderByDescending(patient => patient.createTime).ToList();
                break;
            
            case SortingType.NameAsc:
                patients = patients.OrderBy(patient => patient.name).ToList();
                break;
            
            case SortingType.NameDesc:
                patients = patients.OrderByDescending(patient => patient.name).ToList();
                break;
            
            case SortingType.InspectionAsc:
                
                patients = patients
                    .OrderBy(patient => lastInspections[patient.id]) 
                    .ToList();
                break;
            
            case SortingType.InspectionDesc:
                
                patients = patients
                    .OrderByDescending(patient => lastInspections[patient.id]) 
                    .ToList();
                break;
        }

        return patients;
    }

    private async Task<List<Patient>> FilterPatientsByConclusion(List<Patient> patients, List<Conclusion> conclusions)
    {
        var allInspections = await inspectionRepository.GetAllInspections();
        var patientsWithCurrentConclusion = allInspections
            .Where(i => conclusions.Contains(i.conclusion))
            .Select(i => i.patient.id)
            .Distinct()
            .ToList();

        return patients
            .Where(p => patientsWithCurrentConclusion.Contains(p.id))
            .ToList();
    }

    private async Task<List<Patient>> FilterBySheduledVisits(List<Patient> patients)
    {
        var allInspections = await inspectionRepository.GetAllInspections();
        var patientsWithScheduledVisits = allInspections
            .Where(i => i.nextVisitDate != null && i.nextVisitDate > DateTime.UtcNow)
            .Select(i => i.patient.id)
            .Distinct()
            .ToList();

       return patients
            .Where(p => patientsWithScheduledVisits.Contains(p.id))
            .ToList();
    }

    private async Task<List<Patient>> FilterPatientsByDoctor(List<Patient> patients, Guid doctorId)
    {
        var inspections = await inspectionRepository.GetDoctorInspections(doctorId);
        var patientIdsWithInspections = inspections.Select(inspection => inspection.patient.id).Distinct().ToList();

        return patients
            .Where(patient => patientIdsWithInspections.Contains(patient.id))
            .ToList();
    }
}