using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class TokenRepository : ITokenRepository
{
    private readonly ApplicationDbContext _context;

    public TokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Token?> GetById(Guid id)
    {
        return await _context.Tokens.FindAsync(id);
    }

    public async Task Add(Token token)
    {
        await _context.Tokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpired()
    {
        var expiredTokens = await _context.Tokens
            .Where(t => t.expiresAt <= DateTime.UtcNow)
            .ToListAsync(); 

        _context.Tokens.RemoveRange(expiredTokens); 

        await _context.SaveChangesAsync();       
    }
}