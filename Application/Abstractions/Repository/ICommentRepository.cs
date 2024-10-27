using Domain;

namespace Application.Abstractions.Repository;

public interface ICommentRepository
{
    Task Create(Comment comment);
}