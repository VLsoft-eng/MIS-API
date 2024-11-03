using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class PatientRepository(ApplicationDbContext context) : IPatientRepository
{
    public async Task<Guid> Create(Patient patient)
    {
        await context.AddAsync(patient);
        await context.SaveChangesAsync();
        return patient.id;
    }

    public async Task<Patient?> GetById(Guid id)
    {
        return await context.Patients.FindAsync(id);
    }

    public async Task<List<Patient>> GetAllPatients()
    {
        return await context.Patients.ToListAsync();
    }
}