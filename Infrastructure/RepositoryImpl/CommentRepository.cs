using Application.Abstractions.Repository;
using Domain;

namespace Infrastructure.RepositoryImpl;

public class CommentRepository :ICommentRepository
{
    private readonly ApplicationDbContext _context;

    public CommentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(Comment comment)
    {
        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
    }
}