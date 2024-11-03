using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class DoctorRepository(ApplicationDbContext context) : IDoctorRepository
{
    public async Task Create(Doctor doctor)
    {
        await context.Doctors.AddAsync(doctor);
        await context.SaveChangesAsync();
    }

    public async Task Update(Doctor doctor)
    {
        context.Entry(doctor).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }

    public async Task<Doctor?> GetById(Guid id)
    {
        return await context.Doctors
            .Include(d => d.speciality)
            .FirstOrDefaultAsync(i => i.id == id);
    }

    public async Task<Doctor?> GetByEmail(string email)
    {
        return await context.Doctors
            .FirstOrDefaultAsync(d => d.email == email);
    }
}