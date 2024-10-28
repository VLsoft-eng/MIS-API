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
    }
}