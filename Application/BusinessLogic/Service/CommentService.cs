using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class CommentService : ICommentService
{
    private readonly ICommentMapper _commentMapper;
    private readonly IValidator<CommentEditRequest> _commentEditValidator;
    private readonly IValidator<ConsultationCommentCreateRequest> _consultationCommentCreateValidator;
    private readonly ICommentRepository _commentRepository;
    private readonly IConsultationRepository _consultationRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorMapper _doctorMapper;
    
    
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

    public async Task<List<CommentDto>> GetCommentsByConsultationId(Guid consultationId)
    {
        var comments = await _commentRepository.GetCommentsByConsultationId(consultationId);
        return comments.Select(c => _commentMapper.ToDto(c)).ToList();
    }

    public async Task<InspectionCommentDto> GetRootCommentByConsultation(Guid consultationId)
    {
        var comments = await _commentRepository.GetCommentsByConsultationId(consultationId);
        var rootComment = comments
            .Where(c => c.parent == null)
            .ToList()
            .First();

        var author = _doctorMapper.ToDto(rootComment.author);
        return _commentMapper.ToInspectionCommentDto(rootComment, author);
    }

    public async Task<int> GetCommentsCountByConsultation(Guid consultationId)
    {
        var comments = await GetCommentsByConsultationId(consultationId);
        return comments.Count;
    }
}