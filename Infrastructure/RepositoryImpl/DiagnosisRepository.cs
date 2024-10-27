using Domain;

namespace Infrastructure.RepositoryImpl;

public class DiagnosisRepository
{
    private readonly ApplicationDbContext _context;

    public DiagnosisRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(Diagnosis diagnosis)
    {
        await _context.Diagnoses.AddAsync(diagnosis);
        await _context.SaveChangesAsync();
    }
}