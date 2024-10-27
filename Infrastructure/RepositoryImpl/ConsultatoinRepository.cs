using Application.Abstractions.Repository;
using Domain;

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
}