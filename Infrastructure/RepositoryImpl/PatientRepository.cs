using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Create(Patient patient)
    {
        await _context.AddAsync(patient);
        await _context.SaveChangesAsync();
        return patient.id;
    }

    public async Task<Patient?> GetById(Guid id)
    {
        return await _context.Patients.FindAsync(id);
    }

    public async Task<List<Patient>> GetAllPatients()
    {
        return await _context.Patients.ToListAsync();
    }
}