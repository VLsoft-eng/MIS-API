using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class ConsultationRepository(ApplicationDbContext context) : IConsultationRepository
{
    public async Task Create(Consultation consultation)
    {
        await context.Consultations.AddAsync(consultation);
        await context.SaveChangesAsync();
    }

    public async Task<Consultation?> GetById(Guid consultationId)
    {
        return await context.Consultations
            .Include(c => c.speciality)
            .Include(c => c.inspection)
            .FirstOrDefaultAsync(c => c.id == consultationId);
    }

    public async Task<List<Consultation>> GetBySpecialityId(Guid id)
    {
        return await context.Consultations
            .Include(c => c.speciality)
            .Include(c => c.inspection)
            .Where(c => c.speciality.id == id)
            .ToListAsync();
    }

    public async Task<List<Consultation>> GetByInspectionId(Guid id)
    {
        return await context.Consultations
            .Include(c => c.speciality)
            .Include(c => c.inspection)
            .Where(c => c.inspection.id == id)
            .ToListAsync();
    }
}