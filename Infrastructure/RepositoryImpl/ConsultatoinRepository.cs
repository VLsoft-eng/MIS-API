using Domain;

namespace Infrastructure.RepositoryImpl;

public class ConsultationRepository
{
    private readonly ApplicationDbContext _context;

    public ConsultatoinRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(Consultation consultation)
    {
        await _context.Consultations.AddAsync(consultation);
        await _context.SaveChangesAsync();
    }
}