using Application.Abstractions.Repository;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class InspectionRepository : IInspectionRepository
{
    private readonly ApplicationDbContext _context;

    public InspectionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(Inspection inspection)
    {
        await _context.Inspections.AddAsync(inspection);
        await _context.SaveChangesAsync();
    }

    public async Task<Inspection?> GetById(Guid id)
    {
        return await _context.Inspections.FindAsync(id);
    }

    public async Task<Inspection?> GetBaseInspection(Guid id)
    {
        var inspection = await GetById(id);

        if (inspection == null)
        {
            return null;
        }

        var baseInspection = inspection;

        while (inspection.previousInspection != null)
        {
            baseInspection = inspection.previousInspection;
            inspection = inspection.previousInspection;
        }

        return baseInspection;
    }

    public async Task Update(Inspection inspection)
    {
        _context.Entry(inspection).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task<List<Inspection>> GetRootInspectionsByRequest(Guid patientId, string request)
    {
        var inspections = await _context.Diagnoses
            .Where(d => d.inspection.patient.id == patientId && 
                        d.diagnosisType == DiagnosisType.Main && 
                        (d.icd.name.Contains(request) || d.icd.сode.Contains(request)) &&
                        d.inspection.previousInspection == null)
            .Select(d => d.inspection)
            .ToListAsync();

        return inspections;
    }

    public async Task<List<Inspection>> GetInspectionsByParams(
        Guid patientId, 
        bool grouped, 
        List<string> icd, 
        int page,
        int size)
    {
        var baseQuery = _context.Diagnoses
            .Where(d => d.inspection.patient.id == patientId);

        if (icd != null && icd.Any())
        {
            baseQuery = baseQuery
                .Where(d => d.diagnosisType == DiagnosisType.Main 
                            && icd.Any(root => IsHasEqualRoot(d.icd, root)));
        }

        var inspectionQuery= baseQuery
            .Select(d => d.inspection)
            .Distinct();

        var inspections = await inspectionQuery
            .Include(i => i.patient)
            .Include(i => i.doctor)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
        return inspections;
    }

    private bool IsHasEqualRoot(Icd icd, string code)
    {
        while (icd != null)
        {
            if (icd.сode == code)
            {
                return true;
            }

            icd = icd.parent;
        }

        return false;
    }
}