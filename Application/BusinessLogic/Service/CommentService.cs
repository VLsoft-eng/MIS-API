using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class CommentService(
    ICommentMapper commentMapper,
    IValidator<CommentEditRequest> commentEditValidator,
    IValidator<ConsultationCommentCreateRequest> consultationCommentCreateValidator,
    ICommentRepository commentRepository,
    IConsultationRepository consultationRepository,
    IDoctorRepository doctorRepository,
    IDoctorMapper doctorMapper)
    : ICommentService
{
    public async Task UpdateComment(Guid commentId, CommentEditRequest request)
    {
        var comment = await commentRepository.GetById(commentId);
        if (comment == null)
        {
            throw new CommentNotFoundException();
        }

        if (comment.content == request.content)
        {
            throw new ValidationException("Content not changed");
        }

        var validation = await commentEditValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }
        
        commentMapper.UpdateCommentEntity(comment, request);
        await commentRepository.Update(comment);
    }

    public async Task<Guid> CreateComment(Guid consultationId, Guid doctorId, ConsultationCommentCreateRequest request)
    {
        var consultation = await consultationRepository.GetById(consultationId);
        if (consultation == null)
        {
            throw new ConsultationNotFoundException();
        }

        var validation = await consultationCommentCreateValidator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors[0].ErrorMessage);
        }

        Comment parentComment = null;

        if (request.parentId != null)
        {
            parentComment = await commentRepository.GetById(request.parentId.Value);
            if (parentComment == null)
            {
                throw new CommentNotFoundException("Parent comment not found.");
            }
        }

        var doctor = await doctorRepository.GetById(doctorId);

        Comment comment = commentMapper.ToEntity(request, doctor, consultation, parentComment);
        await commentRepository.Create(comment);
        
        return comment.id;
    }

    public async Task<List<CommentDto>> GetCommentsByConsultationId(Guid consultationId)
    {
        var comments = await commentRepository.GetCommentsByConsultationId(consultationId);
        return comments.Select(c => commentMapper.ToDto(c)).ToList();
    }

    public async Task<InspectionCommentDto> GetRootCommentByConsultation(Guid consultationId)
    {
        var comments = await commentRepository.GetCommentsByConsultationId(consultationId);
        var rootComment = comments
            .Where(c => c.parent == null)
            .ToList()
            .First();

        var author = doctorMapper.ToDto(rootComment.author);
        return commentMapper.ToInspectionCommentDto(rootComment, author);
    }

    public async Task<int> GetCommentsCountByConsultation(Guid consultationId)
    {
        var comments = await GetCommentsByConsultationId(consultationId);
        return comments.Count;
    }
}