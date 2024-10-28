using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class ConsultationRepository : IConsultationRepository
{
    private readonly ApplicationDbContext _context;

    public ConsultationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(Consultation consultation)
    {
        await _context.Consultations.AddAsync(consultation);
        await _context.SaveChangesAsync();
    }

    public async Task<Consultation?> GetById(Guid consultationId)
    {
        return await _context.Consultations
            .Include(c => c.speciality)
            .Include(c => c.inspection)
            .FirstOrDefaultAsync(c => c.id == consultationId);
    }
}