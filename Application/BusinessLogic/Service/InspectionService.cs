using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.BusinessLogic.Validation;
using Application.Dto;
using Application.Exceptions;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class InspectionService : IInspectionService
{
    private readonly IInspectionRepository _inspectionRepository;
    private readonly IConsultationRepository _consultationRepository;
    private readonly IDoctorMapper _doctorMapper;
    private readonly IPatientMapper _patientMapper;
    private readonly IConsultationMapper _consultationMapper;
    private readonly ISpecialityMapper _specialityMapper;
    private readonly ICommentService _commentService;
    private readonly IInspectionMapper _inspectionMapper;
    private readonly IValidator<InspectionEditRequest> _inspectionEditValidator;
    private readonly IIcdRepository _icdRepository;
    private readonly IDiagnosisService _diagnosisService;
    private readonly IDoctorRepository _doctorRepository;
    private IDiagnosisRepository _diagnosisRepository;
    
    public InspectionService(
        IInspectionRepository inspectionRepository,
        IConsultationRepository consultationRepository,
        IDoctorMapper doctorMapper,
        IPatientMapper patientMapper,
        IConsultationMapper consultationMapper,
        ISpecialityMapper specialityMapper,
        ICommentService commentService,
        IInspectionMapper inspectionMapper,
        IValidator<InspectionEditRequest> inspectionEditValidator,
        IIcdRepository icdRepository,
        IDiagnosisService diagnosisService,
        IDoctorRepository doctorRepository,
        IDiagnosisRepository diagnosisRepository)
    {
        _inspectionRepository = inspectionRepository;
        _consultationRepository = consultationRepository;
        _doctorMapper = doctorMapper;
        _patientMapper = patientMapper;
        _consultationMapper = consultationMapper;
        _specialityMapper = specialityMapper;
        _inspectionMapper = inspectionMapper;
        _inspectionEditValidator = inspectionEditValidator;
        _icdRepository = icdRepository;
        _diagnosisService = diagnosisService;
        _doctorRepository = doctorRepository;
        _diagnosisRepository = diagnosisRepository;
        _commentService = commentService;
    }

    public async Task<InspectionDto> GetInspectionById(Guid id)
    {
        var inspection = await _inspectionRepository.GetById(id);
        if (inspection == null)
        {
            throw new InspectionNotFoundException();
        }
        
        var consultations = await _consultationRepository.GetByInspectionId(id);
        var baseInspection = await _inspectionRepository.GetBaseInspection(id);
        var patient = inspection.patient;
        var doctor = inspection.doctor;

        var doctorDto = _doctorMapper.ToDto(doctor);
        var patientDto = _patientMapper.ToDto(patient);
        var diagnosisDtos = await _diagnosisService.GetDiagnosesByInspectionId(inspection.id);
        var consultationDtos = new List<InspectionConsultationDto>();
        foreach (var consultation in consultations)
        {
            var rootCommentDto = await _commentService.GetRootCommentByConsultation(consultation.id);
            var consultationSpecialityDto = _specialityMapper.ToDto(consultation.speciality);
            var commentsCount =  await _commentService.GetCommentsCountByConsultation(consultation.id);
            
            var consultationDto = _consultationMapper.ToInspectionConsultationDto(
                consultation,
                consultationSpecialityDto,
                rootCommentDto,
                consultation.inspection,
                commentsCount);
            consultationDtos.Add(consultationDto);
        }
        
        return _inspectionMapper.ToDto(inspection, baseInspection == null ? null : baseInspection.id, patientDto, doctorDto, diagnosisDtos,
            consultationDtos);
    }

    public async Task EditInspection(Guid inspectionId, InspectionEditRequest request, Guid doctorId)
    {
        var inspection = await _inspectionRepository.GetById(inspectionId);
        if (inspection == null)
        {
            throw new InspectionNotFoundException();
        }

        if (inspection.doctor.id != doctorId)
        {
            throw new DoesntHaveRightsException();
        }

        var validation = await _inspectionEditValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }

        await _diagnosisService.DeleteDiagnosesByInspectionId(inspection.id);
        var newDiagnoses = request.diagnoses;
        foreach (var diagnosis in newDiagnoses)
        {
            var icd = await _icdRepository.GetById(diagnosis.icdDiagnosisId);
            if (icd == null)
            {
                throw new IcdNotFoundException();
            }

            await _diagnosisService.CreateDiagnosis(diagnosis, inspection, icd);
        }
        
        _inspectionMapper.UpdateInspectionEntity(inspection, request);
        await _inspectionRepository.Update(inspection);
    }

    public async Task<List<InspectionFullDto>> GetChainByRoot(Guid rootId)
    {
        var rootInspection = await _inspectionRepository.GetById(rootId);
        if (rootInspection == null)
        {
            throw new InspectionNotFoundException();
        }

        var chain = await _inspectionRepository.GetChainByRoot(rootId);

        var inspectionFullDtos = new List<InspectionFullDto>();
        
        foreach (var inspection in chain)
        {
            bool hasNested = await _inspectionRepository.IsHasChild(inspection.id);
            bool hasChain = hasNested || inspection.previousInspection != null;

            var mainDiagnosisDto = await _diagnosisService.GetMainDIagnosisByInspectionId(inspection.id);
            
            var inspectionFullDto = _inspectionMapper.ToInspectionFullDto(inspection, mainDiagnosisDto, hasChain, hasNested);
            inspectionFullDtos.Add(inspectionFullDto);
        }

        return inspectionFullDtos;
    }
    
    public async Task<InspectionPagedListDto> GetInspectionsWithDoctorSpeciality(
        Guid doctorId,
        bool grouped,
        List<Guid> icdRoots,
        int page,
        int size)
    {
        if (page <= 0 || size <= 0)
        {
            throw new InvalidPaginationParamsException();
        }
        
        var diagnoses = await _diagnosisRepository.GetAllDiagnoses();
        if (icdRoots.Any())
        {
           diagnoses = await _diagnosisService.FilterDiagnosisByIcdRoots(diagnoses, icdRoots);
        }
        
        var inspections = diagnoses.Select(d => d.inspection).Distinct().ToList();

        if (grouped)
        {
            var filteredInspections = new List<Inspection>();
            foreach (var inspection in inspections)
            {
                var inspectionEntity = await _inspectionRepository.GetById(inspection.id);
                if (inspectionEntity.previousInspection == null)
                {
                    filteredInspections.Add(inspectionEntity);
                }
            }

            inspections = filteredInspections;
        }

        var doctor = await _doctorRepository.GetById(doctorId);
        var doctorSpecialtyId = doctor.speciality.id;
        
        var consultations = await _consultationRepository.GetBySpecialityId(doctorSpecialtyId);
        var inspectionIds = consultations.Select(c => c.inspection.id).Distinct().ToList();
        
        inspections = inspections.Where(i => inspectionIds.Contains(i.id)).ToList();
        
        var pagedInspections = inspections
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
        
        List<InspectionFullDto> inspectionFullDtos = new List<InspectionFullDto>();
        foreach (var inspection in pagedInspections)
        {
            bool hasNested = await _inspectionRepository.IsHasChild(inspection.id);
            bool hasChain = (hasNested || inspection.previousInspection != null);

            var mainDiagnosesDto = await _diagnosisService.GetMainDIagnosisByInspectionId(inspection.id);
            var inspectionFullDto =
                _inspectionMapper.ToInspectionFullDto(inspection, mainDiagnosesDto, hasChain, hasNested);
            inspectionFullDtos.Add(inspectionFullDto);
        }

        var overAllInspectionsCount = pagedInspections.Count;
        var totalPages = (int)Math.Ceiling((double)overAllInspectionsCount / size);
        var pageInfo = new PageInfoDto(size, totalPages, page);

        return new InspectionPagedListDto(inspectionFullDtos, pageInfo);
    }
}