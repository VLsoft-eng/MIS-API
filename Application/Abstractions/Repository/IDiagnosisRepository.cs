using Domain;

namespace Application.Abstractions.Repository;

public interface IDiagnosisRepository
{
    Task Create(Diagnosis diagnosis);
    Task<Diagnosis> GetMainDiagnosesByInspectionId(Guid inspectionId);
    Task<List<Diagnosis>> GetPatientsDiagnoses(Guid patientId);
    Task<List<Diagnosis>> GetAllDiagnoses();
    Task<List<Diagnosis>?> GetDiagnosesByInspectionId(Guid id);
    Task DeleteAllDiagnosesByInspection(Guid inspectionId);
    Task<List<Diagnosis>> GetAllMainDiagnoses();
}