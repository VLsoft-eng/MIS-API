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

    public async Task<List<Diagnosis>> GetPatientsDiagnoses(Guid patientId)
    {
        return await _context.Diagnoses
            .Include(d => d.icd)
            .Include(d => d.inspection)
            .Include(d => d.inspection.patient)
            .Include(d => d.inspection.previousInspection)
            .Include(d => d.inspection.doctor)
            .Where(d => d.inspection.patient.id == patientId)
            .ToListAsync();
    }

    public async Task<List<Diagnosis>> GetAllDiagnoses()
    {
        return await _context.Diagnoses
            .Include(d => d.inspection)
            .Include(d => d.inspection.patient)
            .Include(d => d.icd)
            .ToListAsync();
    }
}