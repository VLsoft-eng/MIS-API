using Application.Abstractions.Repository;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class DiagnosisRepository(ApplicationDbContext context) : IDiagnosisRepository
{
    public async Task Create(Diagnosis diagnosis)
    {
        await context.Diagnoses.AddAsync(diagnosis);
        await context.SaveChangesAsync();
    }

    public async Task<Diagnosis> GetMainDiagnosesByInspectionId(Guid inspectionId)
    {
        var diagnoses =  await context.Diagnoses
            .Include(d => d.icd) 
            .Where(d => d.inspection.id == inspectionId)
            .Where(d => d.diagnosisType == DiagnosisType.Main)
            .ToListAsync();
        return diagnoses[0];
    }

    public async Task<List<Diagnosis>?> GetDiagnosesByInspectionId(Guid id)
    {
        return await context.Diagnoses
            .Include(d => d.icd)
            .Include(d => d.inspection)
            .Where(d => d.inspection.id == id)
            .ToListAsync();
    }

    public async Task<List<Diagnosis>> GetPatientsDiagnoses(Guid patientId)
    {
        return await context.Diagnoses
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
        return await context.Diagnoses
            .Include(d => d.inspection)
            .Include(d => d.inspection.patient)
            .Include(d => d.icd)
            .ToListAsync();
    }

    public async Task DeleteAllInspectionDiagnoses(Guid inspectionId)
    {
        var diagnosesToDelete = await context.Diagnoses
            .Include(d => d.inspection)
            .Where(d => d.inspection.id == inspectionId)
            .ToListAsync();

        if (diagnosesToDelete.Any())
        {
            context.Diagnoses.RemoveRange(diagnosesToDelete);
            await context.SaveChangesAsync();
        }
    }

    public async Task CreateRange(List<Diagnosis> diagnoses)
    {
        await context.Diagnoses.AddRangeAsync(diagnoses);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAllDiagnosesByInspection(Guid inspectionId)
    {
        var diagnosesToDelete = await context.Diagnoses
            .Include(d => d.inspection)
            .Where(d => d.inspection.id == inspectionId)
            .ToListAsync();

        context.Diagnoses.RemoveRange(diagnosesToDelete);
        await context.SaveChangesAsync();
    }
}