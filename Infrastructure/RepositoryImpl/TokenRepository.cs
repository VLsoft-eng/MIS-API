using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class TokenRepository(ApplicationDbContext context) : ITokenRepository
{
    public async Task<Token?> GetById(Guid id)
    {
        return await context.Tokens.FindAsync(id);
    }

    public async Task Add(Token token)
    {
        await context.Tokens.AddAsync(token);
        await context.SaveChangesAsync();
    }

    public async Task DeleteExpired()
    {
        var expiredTokens = await context.Tokens
            .Where(t => t.expiresAt <= DateTime.UtcNow)
            .ToListAsync(); 

        context.Tokens.RemoveRange(expiredTokens); 

        await context.SaveChangesAsync();       
    }
}