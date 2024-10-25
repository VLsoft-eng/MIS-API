using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class SpecialityRepository : ISpecialityRepository
{
    private readonly ApplicationDbContext _context;

    public SpecialityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(Speciality speciality)
    {
        await _context.Specialities.AddAsync(speciality);
        await _context.SaveChangesAsync();
    }

    public async Task<Speciality?> GetById(Guid id)
    {
        return await _context.Specialities.FindAsync(id);
    }

    public async Task<IEnumerable<Speciality>> GetByNameAndParams(string name, int page, int size)
    {
        return await _context.Specialities
            .Where(s => s.name.Contains(name))
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
    }
}