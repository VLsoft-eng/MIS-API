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
        List<Guid> icd, 
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

        var inspectionQuery = baseQuery
            .Select(d => d.inspection)
            .Distinct();
            
        if (grouped)
        {
            inspectionQuery = inspectionQuery.Where(i => i.previousInspection  == null);
        }

        var inspections = await inspectionQuery
            .Include(i => i.patient)
            .Include(i => i.doctor)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
        return inspections;
    }

    public async Task<int> GetInspectionsCountByParams(
        Guid patientId,
        bool grouped,
        List<Guid> icd)
    {
        var baseQuery = _context.Diagnoses
            .Where(d => d.inspection.patient.id == patientId);

        if (icd != null && icd.Any())
        {
            baseQuery = baseQuery
                .Where(d => d.diagnosisType == DiagnosisType.Main 
                            && icd.Any(root => IsHasEqualRoot(d.icd, root)));
        }

        var inspectionQuery = baseQuery
            .Select(d => d.inspection)
            .Distinct();
            
        if (grouped)
        {
            inspectionQuery = inspectionQuery.Where(i => i.previousInspection  == null);
        }

        return await inspectionQuery.CountAsync();
    }

    private bool IsHasEqualRoot(Icd icd, Guid id)
    {
        while (icd != null)
        {
            if (icd.id == id)
            {
                return true;
            }

            icd = icd.parent;
        }

        return false;
    }

    public async Task<bool> IsHasChild(Guid id)
    {
        return await _context.Inspections.AnyAsync(i => i.previousInspection.id != null
                                           && i.previousInspection.id == id);
    }
}