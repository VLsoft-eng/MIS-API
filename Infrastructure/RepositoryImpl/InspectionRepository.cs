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
        return await _context.Inspections
            .Include(i => i.doctor)
            .Include(i => i.patient)
            .Include(i => i.previousInspection)
            .FirstOrDefaultAsync(i => i.id == id);
    }

    public async Task<Inspection?> GetBaseInspection(Guid id)
    {
        var inspection = await GetById(id);

        if (inspection == null)
        {
            return null;
        }

        var baseInspection = inspection;

        while (baseInspection.previousInspection != null)
        {
            baseInspection = await GetById(baseInspection.previousInspection.id);
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

    public async Task<List<Inspection>> GetPatientInspections(Guid patientId)
    {
        return await _context.Inspections
            .Where(i => i.patient.id == patientId)
            .ToListAsync();
    }

    public async Task<List<Inspection>> GetDoctorInspections(Guid doctorId)
    {
        return await _context.Inspections
            .Include(i => i.doctor)
            .Where(i => i.doctor.id == doctorId)
            .ToListAsync();
    }

    public async Task<List<Inspection>> GetAllInspections()
    {
        return await _context.Inspections
            .Include(i => i.patient)
            .ToListAsync();
    }

    public async Task<bool> IsHasChild(Guid id)
    {
        return await _context.Inspections
            .Include(i => i.previousInspection)
            .AnyAsync(i => i.previousInspection.id != null && i.previousInspection.id == id);
    }

    public async Task<List<Inspection>> GetChainByRoot(Guid rootId)
    {
        var allInspections = await _context.Inspections
            .Include(i => i.previousInspection)
            .ToListAsync();

        var chain = new List<Inspection>();
        var currentInspection = allInspections.FirstOrDefault(i => i.id == rootId);
        
        while (currentInspection != null)
        {
            chain.Add(currentInspection);
            currentInspection = allInspections
                .FirstOrDefault(i => i.id == currentInspection.previousInspection?.id);
        }

        return chain;
    }

    public async Task<List<Inspection>> GetInspectionsInTimeInterval(DateTime start, DateTime end)
    {
        return await _context.Inspections
            .Include(i => i.patient)
            .Where(i => i.date >= start && i.date <= end)
            .ToListAsync();
    }

    public async Task<List<Inspection>> GetMissedInspections()
    {
        return await _context.Inspections
            .Where(i =>
                i.date <= DateTime.UtcNow &&
                i.conclusion != Conclusion.Death &&
                i.nextVisitDate == null)
            .Include(i => i.doctor)
            .Include(i => i.patient)
            .ToListAsync();
    }
}