using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class SpecialityRepository(ApplicationDbContext context) : ISpecialityRepository
{
    public async Task Create(Speciality speciality)
    {
        await context.Specialities.AddAsync(speciality);
        await context.SaveChangesAsync();
    }

    public async Task<Speciality?> GetById(Guid id)
    {
        return await context.Specialities.FindAsync(id);
    }

    public async Task<List<Speciality>> GetAllSpecialities()
    {
        return await context.Specialities.ToListAsync();
    }
}