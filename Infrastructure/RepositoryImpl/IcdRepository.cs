using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class IcdRepository(ApplicationDbContext context) : IIcdRepository
{
    public async Task <List<Icd>> GetRootElements()
    {
        return await context.Icds
            .Where(icd => icd.parent == null)
            .ToListAsync();
    }

    public async Task<List<Icd>> GetAllIcds()
    {
        return await context.Icds
            .Include(i => i.parent)
            .ToListAsync();
    }

    public async Task<Icd?> GetById(Guid id)
    {
        return await context.Icds
            .Include(i => i.parent)
            .FirstOrDefaultAsync(i => i.id == id);
    }

    public async Task<Icd> GetRootByIcdId(Guid id)
    {
        var icd = await context.Icds
            .Include(i => i.parent)
            .Where(i => i.id == id)
            .FirstOrDefaultAsync();

        while (icd != null)
        {
            if (icd.parent == null)
            {
                return icd;
            }

            icd = await GetById(icd.parent.id);
        }

        return icd;
    } 
    
}