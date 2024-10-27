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
        List<string> icd,
        int page,
        int size);
}