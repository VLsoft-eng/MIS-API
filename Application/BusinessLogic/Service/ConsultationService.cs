using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class ConsultationService : IConsultationService
{
    private readonly IConsultationRepository _consultationRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IInspectionRepository _inspectionRepository;
    private readonly ICommentMapper _commentMapper;
    private readonly IValidator<CommentEditRequest> _commentEditValidator;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IValidator<ConsultationCommentCreateRequest> _consultationCommentCreateValidator;
    private readonly ISpecialityMapper _specialityMapper;
    private readonly IConsultationMapper _consultationMapper;
    private readonly IDiagnosisRepository _diagnosisRepository;
    private readonly IDiagnosisMapper _diagnosisMapper;
    private readonly IInspectionMapper _inspectionMapper;
    private readonly IIcdRepository _icdRepository;

    public ConsultationService(
        IConsultationRepository consultationRepository,
        IInspectionRepository inspectionRepository,
        ICommentRepository commentRepository,
        ICommentMapper commentMapper,
        IValidator<CommentEditRequest> commentEditValidator,
        IDoctorRepository doctorRepository,
        IValidator<ConsultationCommentCreateRequest> consultationCommentCreateValidator,
        ISpecialityMapper specialityMapper,
        IConsultationMapper consultationMapper,
        IDiagnosisRepository diagnosisRepository,
        IDiagnosisMapper diagnosisMapper,
        IInspectionMapper inspectionMapper,
        IIcdRepository icdRepository)
    {
        _consultationRepository = consultationRepository;
        _inspectionRepository = inspectionRepository;
        _commentRepository = commentRepository;
        _commentMapper = commentMapper;
        _commentEditValidator = commentEditValidator;
        _doctorRepository = doctorRepository;
        _consultationCommentCreateValidator = consultationCommentCreateValidator;
        _specialityMapper = specialityMapper;
        _consultationMapper = consultationMapper;
        _diagnosisRepository = diagnosisRepository;
        _diagnosisMapper = diagnosisMapper;
        _inspectionMapper = inspectionMapper;
        _icdRepository = icdRepository;
    }

    public async Task UpdateComment(Guid commentId, CommentEditRequest request)
    {
        var comment = await _commentRepository.GetById(commentId);
        if (comment == null)
        {
            throw new CommentNotFoundException();
        }

        var validation = await _commentEditValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }
        
        _commentMapper.UpdateCommentEntity(comment, request);
        await _commentRepository.Update(comment);
    }

    public async Task<Guid> CreateComment(Guid consultationId, Guid doctorId, ConsultationCommentCreateRequest request)
    {
        var consultation = await _consultationRepository.GetById(consultationId);
        if (consultation == null)
        {
            throw new ConsultationNotFoundException();
        }

        var validation = await _consultationCommentCreateValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }

        Comment parentComment = null;

        if (request.parentId != null)
        {
            parentComment = await _commentRepository.GetById(request.parentId.Value);
            if (parentComment == null)
            {
                throw new CommentNotFoundException("Parent comment not found.");
            }
        }

        var doctor = await _doctorRepository.GetById(doctorId);

        Comment comment = _commentMapper.ToEntity(request, doctor, consultation, parentComment);
        await _commentRepository.Create(comment);
        
        return comment.id;
    }

    public async Task<ConsultationDto> GetConsultation(Guid consultationId)
    {
        var consultation = await _consultationRepository.GetById(consultationId);
        if (consultation == null)
        {
            throw new ConsultationNotFoundException();
        }

        var comments = await _commentRepository.GetCommentsByConsultationId(consultationId);
        
        var commentDtos = comments.Select(c => _commentMapper.ToDto(c)).ToList();
        var specialityDto = _specialityMapper.ToDto(consultation.speciality);

        var consultationDto = _consultationMapper.ToDto(consultation, specialityDto, commentDtos);
        return consultationDto;
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
            await FilterDiagnosisByIcdRoots(diagnoses, icdRoots);
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

    private async Task FilterDiagnosisByIcdRoots(List<Diagnosis> diagnoses, List<Guid> icdRoots)
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

        diagnoses = filteredDiagnoses;
    }
}