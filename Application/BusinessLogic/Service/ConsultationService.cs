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

    public ConsultationService(
        IConsultationRepository consultationRepository,
        IInspectionRepository inspectionRepository,
        ICommentRepository commentRepository,
        ICommentMapper commentMapper,
        IValidator<CommentEditRequest> commentEditValidator,
        IDoctorRepository doctorRepository,
        IValidator<ConsultationCommentCreateRequest> consultationCommentCreateValidator,
        ISpecialityMapper specialityMapper,
        IConsultationMapper consultationMapper)
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
}