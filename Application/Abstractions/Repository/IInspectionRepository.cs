using Domain;

namespace Application.Abstractions.Repository;

public interface IInspectionRepository
{
    Task Create(Inspection inspection);
    Task<Inspection?> GetById(Guid id);
    Task<Inspection?> GetBaseInspection(Guid id);
    Task<List<Inspection>> GetRootInspectionsByRequest(Guid patientId, string request);
    Task<List<Inspection>> GetInspectionsByParams(
        Guid patientId,
        bool grouped,
        List<Guid> icd,
        int page,
        int size);
    Task<int> GetInspectionsCountByParams(
        Guid patientId,
        bool grouped,
        List<Guid> icd);
    Task<bool> IsHasChild(Guid id);

}