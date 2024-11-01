using Application.Abstractions.Mapper;
using Application.Dto;
using Domain;

namespace Application.BusinessLogic.Mapper;

public class CommentMapper : ICommentMapper
{
    public Comment ToEntity(
        InspectionCommentCreateRequest request,
        Doctor doctor, 
        Consultation consultation, 
        Comment parent)
    {
        return new Comment
        {
            id = Guid.NewGuid(),
            createTime = DateTime.UtcNow,
            author = doctor,
            consultation = consultation,
            content = request.content,
            modifiedDate = DateTime.UtcNow,
            parent = parent,
        };
    }

    public void UpdateCommentEntity(Comment comment, CommentEditRequest request)
    {
        comment.content = request.content;
        comment.modifiedDate = DateTime.UtcNow;
    }

    public Comment ToEntity(
        ConsultationCommentCreateRequest request,
        Doctor doctor,
        Consultation consultation,
        Comment parent)
    {
        return new Comment
        {
            id = Guid.NewGuid(),
            createTime = DateTime.UtcNow,
            author = doctor,
            consultation = consultation,
            content = request.content,
            modifiedDate = null,
            parent = parent
        };
    }

    public CommentDto ToDto(Comment comment)
    {
        return new CommentDto(
            comment.id,
            comment.createTime,
            comment.modifiedDate,
            comment.content,
            comment.author.id,
            comment.author.name,
            comment.parent == null ? null : comment.parent.id);
    }

    public InspectionCommentDto ToInspectionCommentDto(Comment comment, DoctorDto doctor)
    {
        return new InspectionCommentDto(
            comment.id,
            comment.createTime,
            comment.parent == null ? null : comment.parent.id,
            comment.content,
            doctor, 
            comment.modifiedDate);
    }
}