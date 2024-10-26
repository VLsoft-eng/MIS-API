using Application.Abstractions.Repository;
using Domain;

namespace Infrastructure.RepositoryImpl;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(Patient patient)
    {
        await _context.AddAsync(patient);
        await _context.SaveChangesAsync();
    }

    public async Task<Patient?> GetById(Guid id)
    {
        return await _context.Patients.FindAsync(id);
    }
}