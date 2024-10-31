using Application.Dto;

namespace Application.Abstractions.Service;

public interface ICommentService
{
    Task<Guid> CreateComment(Guid consultationId, Guid doctorId, ConsultationCommentCreateRequest request);
    Task UpdateComment(Guid commentId, CommentEditRequest request);
    Task<List<CommentDto>> GetCommentsByConsultationId(Guid consultationId);
    Task<InspectionCommentDto> GetRootCommentByConsultation(Guid consultationId);
    Task<int> GetCommentsCountByConsultation(Guid consultationId);
}