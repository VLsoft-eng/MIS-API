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

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPatientMapper _patientMapper;
    private readonly IValidator<PatientCreateRequest> _patientCreateValidator;
    private readonly IValidator<InspectionCreateRequest> _inspectionCreateValidator;
    private readonly IValidator<ConsultationCreateRequest> _consultationCreateValidator;
    private readonly IValidator<InspectionCommentCreateRequest> _inspectionCreateCommentValidator;
    private readonly IValidator<DiagnosisCreateRequest> _diagnosisCreateValidator;
    private readonly IIcdRepository _icdRepository;
    private readonly ISpecialityRepository _specialityRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IInspectionRepository _inspectionRepository;
    private readonly IConsultationRepository _consultationRepository;
    private readonly IDiagnosisRepository _diagnosisRepository;
    private readonly IInspectionMapper _inspectionMapper;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDiagnosisMapper _diagnosisMapper;
    private readonly ICommentMapper _commentMapper;
    private readonly IConsultationMapper _consultationMapper;

    public PatientService(
        IPatientRepository patientRepository,
        IPatientMapper patientMapper,
        IValidator<PatientCreateRequest> patientCreateValidator,
        IValidator<InspectionCreateRequest> inspectionCreateValidator,
        IValidator<ConsultationCreateRequest> consultationCreateValidator,
        IValidator<InspectionCommentCreateRequest> inspectionCreateCommentValidator,
        IValidator<DiagnosisCreateRequest> diagnosisCreateValidator,
        IIcdRepository icdRepository,
        ISpecialityRepository specialityRepository,
        ICommentRepository commentRepository,
        IInspectionRepository inspectionRepository,
        IConsultationRepository consultationRepository,
        IDiagnosisRepository diagnosisRepository,
        IInspectionMapper inspectionMapper,
        IDoctorRepository doctorRepository,
        IDiagnosisMapper diagnosisMapper,
        ICommentMapper commentMapper,
        IConsultationMapper consultationMapper)
    {
        _patientRepository = patientRepository;
        _patientMapper = patientMapper;
        _patientCreateValidator = patientCreateValidator;
        _inspectionCreateValidator = inspectionCreateValidator;
        _consultationCreateValidator = consultationCreateValidator;
        _inspectionCreateCommentValidator = inspectionCreateCommentValidator;
        _diagnosisCreateValidator = diagnosisCreateValidator;
        _icdRepository = icdRepository;
        _specialityRepository = specialityRepository;
        _commentRepository = commentRepository;
        _inspectionRepository = inspectionRepository;
        _consultationRepository = consultationRepository;
        _diagnosisRepository = diagnosisRepository;
        _inspectionMapper = inspectionMapper;
        _doctorRepository = doctorRepository;
        _diagnosisMapper = diagnosisMapper;
        _commentMapper = commentMapper;
        _consultationMapper = consultationMapper;
    }

    public async Task Create(PatientCreateRequest request)
    {
        var validation = await _patientCreateValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }

        var patient = _patientMapper.ToEntity(request);
        await _patientRepository.Create(patient);
    }

    public async Task<PatientDto> GetPatientById(Guid id)
    {
        var patient = await _patientRepository.GetById(id);
        if (patient == null)
        {
            throw new PatientNotFoundException();
        }

        var patientDto = _patientMapper.ToDto(patient);
        return patientDto;
    }

    public async Task<Guid> CreatePatientsInspection(Guid patientId, Guid doctorId, InspectionCreateRequest request)
    {
        var patient = await _patientRepository.GetById(patientId);
        if (patient == null)
        {
            throw new PatientNotFoundException();
        }

        var inspectionValidation = await _inspectionCreateValidator.ValidateAsync(request);
        if (!inspectionValidation.IsValid)
        {
            throw new ValidationException(inspectionValidation.Errors[0].ErrorMessage);
        }

        Inspection previousInspection = null;

        if (request.previousInspectionId != null)
        {
            var previousInspectionId = request.previousInspectionId ?? Guid.NewGuid();
            previousInspection = await _inspectionRepository.GetById(previousInspectionId);
            if (previousInspection == null)
            {
                throw new InspectionNotFoundException();
            }
        }

        if (request.consultations != null)
        {
            foreach (var consultation in request.consultations)
            {
                var consultationValidation = await _consultationCreateValidator.ValidateAsync(consultation);
                if (!consultationValidation.IsValid)
                {
                    throw new ValidationException(consultationValidation.Errors[0].ErrorMessage);
                }

                var commentValidation = await _inspectionCreateCommentValidator.ValidateAsync(consultation.comment);
                if (!commentValidation.IsValid)
                {
                    throw new ValidationException(commentValidation.Errors[0].ErrorMessage);
                }

                var speciality = await _specialityRepository.GetById(consultation.specialityId);
                if (speciality == null)
                {
                    throw new SpecialityNotFoundException();
                }
            }
        }

        foreach (var diagnosis in request.diagnoses)
        {
            var diagnosisValidation = await _diagnosisCreateValidator.ValidateAsync(diagnosis);
            if (!diagnosisValidation.IsValid)
            {
                throw new ValidationException(diagnosisValidation.Errors[0].ErrorMessage);
            }

            var icd = await _icdRepository.GetById(diagnosis.icdDiagnosisId);

            if (icd == null)
            {
                throw new IcdNotFoundException();
            }
        }

        var doctor = await _doctorRepository.GetById(doctorId);

        Inspection inspection = _inspectionMapper.ToEntity(request, doctor, patient, previousInspection);

        await _inspectionRepository.Create(inspection);

        foreach (var diagnosis in request.diagnoses)
        {
            Icd icd = await _icdRepository.GetById(diagnosis.icdDiagnosisId);
            var diagnosisEntity = _diagnosisMapper.ToEntity(diagnosis, icd, inspection);
            await _diagnosisRepository.Create(diagnosisEntity);
        }

        if (request.consultations != null)
        {
            foreach (var consultationRequest in request.consultations)
            {
                Speciality speciality = await _specialityRepository.GetById(consultationRequest.specialityId);
                Consultation consultation = _consultationMapper.ToEntity(speciality, inspection);
                Comment comment = _commentMapper.ToEntity(consultationRequest.comment, doctor, consultation, null);

                await _consultationRepository.Create(consultation);
                await _commentRepository.Create(comment);
            }
        }

        return inspection.id;
    }
    
    public async Task<List<InspectionShortDto>> SearchPatientInspectionsByParams(Guid patientId, string request)
    {
        var patient = await _patientRepository.GetById(patientId);
        if (patient == null)
        {
            throw new PatientNotFoundException();
        }

        List <Inspection> inspections = await _inspectionRepository.GetRootInspectionsByRequest(patientId, request);
        
        List<InspectionShortDto> inspectionShortDtos = new List<InspectionShortDto>();
        foreach (var inspection in inspections)
        {
            List<Diagnosis> diagnoses = await _diagnosisRepository.GetMainDiagnosesByInspectionId(inspection.id);
            var mainDiagnosis = diagnoses[0];
            var diagnosisDto = _diagnosisMapper.ToDto(mainDiagnosis);
            
            var inspectionShortDto = _inspectionMapper.ToInspectionShortDto(inspection, diagnosisDto);
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
        var patient = await _patientRepository.GetById(patientId);
        if (patient == null)
        {
            throw new PatientNotFoundException();
        }

        var diagnoses = await _diagnosisRepository.GetPatientsDiagnoses(patientId);
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
        
        var inspections = diagnoses.Select(d => d.inspection).Distinct().ToList();

        if (grouped)
        {
            inspections = inspections
                .Where(i => i.previousInspection == null)
                .ToList();
        }
        
        var pagedInspections = inspections
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
        
        List<InspectionFullDto> inspectionFullDtos = new List<InspectionFullDto>();
        foreach (var inspection in pagedInspections)
        {
            bool hasNested = await _inspectionRepository.IsHasChild(inspection.id);
            bool hasChain = (hasNested || inspection.previousInspection != null);

            var diagnosis = await _diagnosisRepository.GetMainDiagnosesByInspectionId(inspection.id);
            var diagnosisDto = _diagnosisMapper.ToDto(diagnosis[0]);

            var inspectionFullDto =
                _inspectionMapper.ToInspectionFullDto(inspection, diagnosisDto, hasChain, hasNested);
            inspectionFullDtos.Add(inspectionFullDto);
        }

        var overAllInspectionsCount = pagedInspections.Count;
        var totalPages = (int)Math.Ceiling((double)overAllInspectionsCount / size);
        var pageInfo = new PageInfoDto(size, totalPages, page);

        return new InspectionPagedListDto(inspectionFullDtos, pageInfo);
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

    public async Task<PatientPagedListDto> GetPatientsByParams(
        string request,
        Conclusion conclusion,
        SortingType sorting,
        bool scheduledVisits,
        bool onlyMine,
        int page, 
        int size,
        Guid doctorId)
    {
        var patients = await _patientRepository.GetAllPatients();
        patients = patients.Where(patient => patient.name.ToLower().Contains(request.ToLower())).ToList();

        if (onlyMine)
        {
            var inspections = await _inspectionRepository.GetDoctorInspections(doctorId);
            var patientIdsWithInspections = inspections.Select(inspection => inspection.patient.id).Distinct().ToList();

            patients = patients
                .Where(patient => patientIdsWithInspections.Contains(patient.id))
                .ToList();
        }

        if (scheduledVisits)
        {
            var allInspections = await _inspectionRepository.GetAllInspections();
            var patientsWithScheduledVisits = allInspections
                .Where(i => i.nextVisitDate != null && i.nextVisitDate > DateTime.UtcNow)
                .Select(i => i.patient.id)
                .Distinct()
                .ToList();

            patients = patients
                .Where(p => patientsWithScheduledVisits.Contains(p.id))
                .ToList();
        }

        if (conclusion != null)
        {
            var allInspections = await _inspectionRepository.GetAllInspections();
            var patientsWithCurrentConclusion = allInspections
                .Where(i => i.conclusion == conclusion)
                .Select(i => i.patient.id)
                .Distinct()
                .ToList();

            patients = patients
                .Where(p => patientsWithCurrentConclusion.Contains(p.id))
                .ToList();
        }
        
        
        var lastInspectionsDict = new Dictionary<Guid, DateTime?>();
        
        foreach (var patient in patients)
        {
            var inspections = await _inspectionRepository.GetPatientInspections(patient.id); 
            var lastInspectionDate = inspections.Max(i => i.date); 
            lastInspectionsDict[patient.id] = lastInspectionDate;
        }
        

        switch (sorting)
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
                    .OrderBy(patient => lastInspectionsDict[patient.id]) 
                    .ToList();
                break;
            
            case SortingType.InspectionDesc:
                
                patients = patients
                    .OrderByDescending(patient => lastInspectionsDict[patient.id]) 
                    .ToList();
                break;
        }

        var overAllPatientsCount = patients.Count;
        var totalPages = (int)Math.Ceiling((double)overAllPatientsCount / size);

        var pagedPatients = patients
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();

        List<PatientDto> patientDtos = pagedPatients
            .Select(p => _patientMapper.ToDto(p))
            .ToList();
        var pageInfo = new PageInfoDto(size, totalPages, page);
        var patientPagedListDto = new PatientPagedListDto(patientDtos, pageInfo);

        return patientPagedListDto;
    }
}