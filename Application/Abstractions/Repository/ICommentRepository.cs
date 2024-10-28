using Domain;

namespace Application.Abstractions.Repository;

public interface ICommentRepository
{
    Task Create(Comment comment);
    Task Update(Comment comment);
    Task<Comment?> GetById(Guid commentId);
    Task<List<Comment>> GetCommentsByConsultationId(Guid consultationId);
}