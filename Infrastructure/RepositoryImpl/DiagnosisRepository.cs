using Application.Abstractions.Repository;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class DiagnosisRepository : IDiagnosisRepository
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

    public async Task<List<Diagnosis>> GetDiagnosesByInspectionId(Guid inspectionId)
    {
        return await _context.Diagnoses
            .Where(d => d.inspection.id == inspectionId)
            .ToListAsync();
    }
}