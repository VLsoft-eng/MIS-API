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

    public async Task<List<Speciality>> GetAllSpecialities()
    {
        return await _context.Specialities.ToListAsync();
    }
}