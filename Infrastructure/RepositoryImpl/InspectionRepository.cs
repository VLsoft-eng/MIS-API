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
        return await _context.Inspections.AnyAsync(i => i.previousInspection.id != null
                                           && i.previousInspection.id == id);
    }
}