using Domain;

namespace Application.Abstractions.Repository;

public interface IDiagnosisRepository
{
    Task Create(Diagnosis diagnosis);
    Task<List<Diagnosis>> GetMainDiagnosesByInspectionId(Guid inspectionId);
    Task<List<Diagnosis>> GetPatientsDiagnoses(Guid patientId);
    Task<List<Diagnosis>> GetAllDiagnoses();
}