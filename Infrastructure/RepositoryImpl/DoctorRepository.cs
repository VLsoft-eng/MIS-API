using Application.Abstractions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class DoctorRepository : IDoctorRepository
{
    private readonly ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(Doctor doctor)
    {
        await _context.Doctors.AddAsync(doctor);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Doctor doctor)
    {
        _context.Entry(doctor).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<Doctor?> GetById(Guid id)
    {
        return await _context.Doctors.FindAsync(id);
    }

    public async Task<Doctor?> GetByEmail(string email)
    {
        return await _context.Doctors
            .FirstOrDefaultAsync(d => d.email == email);
    }
}