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
    Task<List<Inspection>> GetDoctorInspections(Guid doctorId);
    Task<List<Inspection>> GetAllInspections();
    Task Update(Inspection inspection);
    Task<List<Inspection>> GetChainByRoot(Guid rootId);
    Task<List<Inspection>> GetInspectionsInTimeInterval(DateTime start, DateTime end);
    Task<List<Inspection>> GetMissedInspections();
    Task UpdateIsNotified(Guid inspectionId, bool isNotified);

}