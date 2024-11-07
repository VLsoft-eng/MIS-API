using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class CommentRepository(ApplicationDbContext context) : ICommentRepository
{
    public async Task Create(Comment comment)
    {
        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
    }
    
    public async Task Update(Comment comment)
    {
        context.Entry(comment).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task<Comment?> GetById(Guid commentId)
    {
        return await context.Comments
            .Include(c => c.parent)
            .Include(c => c.author)
            .Include(c => c.consultation)
            .FirstOrDefaultAsync(c => c.id == commentId);
    }

    public async Task<List<Comment>> GetCommentsByConsultationId(Guid consultationId)
    {
        return await context.Comments
            .Include(c => c.author)
            .Include(c => c.parent)
            .Include(c => c.consultation)
            .Include(c => c.author.speciality)
            .Where(c => c.consultation.id == consultationId)
            .ToListAsync();
    }
}