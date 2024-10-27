using Application.Abstractions.Repository;
using Domain;
using Domain.Enums;
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

    public async Task<List<Diagnosis>> GetMainDiagnosesByInspectionId(Guid inspectionId)
    {
        return await _context.Diagnoses
            .Include(d => d.icd) 
            .Where(d => d.inspection.id == inspectionId)
            .Where(d => d.diagnosisType == DiagnosisType.Main)
            .ToListAsync();
    }
}