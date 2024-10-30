using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class IcdRepository : IIcdRepository
{
    private readonly ApplicationDbContext _context;

    public IcdRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task <List<Icd>> GetRootElements()
    {
        return await _context.Icds
            .Where(icd => icd.parent == null)
            .ToListAsync();
    }
    
    public async Task<List<Icd>> GetByNameAndParams(string name, int page, int size)
    {
        return await _context.Icds
            .Where(s => s.name.Contains(name))
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }

    public async Task<int> GetCountByName(string name)
    {
        return await _context.Icds
            .CountAsync(s => s.name.Contains(name));
    }

    public async Task<Icd?> GetById(Guid id)
    {
        return await _context.Icds
            .Include(i => i.parent)
            .FirstOrDefaultAsync(i => i.id == id);
    }

    public async Task<Icd> GetRootByIcdId(Guid id)
    {
        var icd = await _context.Icds
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