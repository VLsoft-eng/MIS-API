using Application.Dto;

namespace Application.Abstractions.Service;

public interface IConsultationService
{

    Task UpdateComment(Guid commentId, CommentEditRequest request);
}