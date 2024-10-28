using Domain;

namespace Application.Abstractions.Repository;

public interface IInspectionRepository
{
    Task Create(Inspection inspection);
    Task<Inspection?> GetById(Guid id);
    Task<Inspection?> GetBaseInspection(Guid id);
    Task<List<Inspection>> GetRootInspectionsByRequest(Guid patientId, string request);
    Task<List<Inspection>> GetPatientInspections(Guid patientId);
    Task<bool> IsHasChild(Guid id);

}