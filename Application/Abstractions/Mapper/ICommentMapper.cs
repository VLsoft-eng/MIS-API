using Application.Dto;
using Domain;

namespace Application.Abstractions.Mapper;

public interface ICommentMapper
{
    Comment ToEntity(
        InspectionCommentCreateRequest request,
        Doctor doctor,
        Consultation consultation,
        Comment parent);

    void UpdateCommentEntity(Comment comment, CommentEditRequest request);

    Comment ToEntity(
        ConsultationCommentCreateRequest request,
        Doctor doctor,
        Consultation consultation,
        Comment parent);

    CommentDto ToDto(Comment comment);
}