using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

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
    
    public async Task Update(Comment comment)
    {
        _context.Entry(comment).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<Comment?> GetById(Guid commentId)
    {
        return await _context.Comments
            .Include(c => c.parent)
            .FirstOrDefaultAsync(c => c.id == commentId);
    }

    public async Task<List<Comment>> GetCommentsByConsultationId(Guid consultationId)
    {
        return await _context.Comments
            .Include(c => c.author)
            .Include(c => c.parent)
            .Include(c => c.consultation)
            .Include(c => c.author.speciality)
            .Where(c => c.consultation.id == consultationId)
            .ToListAsync();
    }
}