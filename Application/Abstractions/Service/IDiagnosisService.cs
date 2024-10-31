using Application.Dto;
using Domain;

namespace Application.Abstractions.Service;

public interface IDiagnosisService
{
    Task CreateDiagnosis(DiagnosisCreateRequest request, Inspection inspection, Icd icd);
    Task<DiagnosisDto> GetMainDIagnosisByInspectionId(Guid inspectionId);
    Task DeleteDiagnosesByInspectionId(Guid inspectionId);
    Task<List<DiagnosisDto>> GetDiagnosesByInspectionId(Guid inspectionId);
    Task<List<Diagnosis>> FilterDiagnosisByIcdRoots(List<Diagnosis> diagnoses, List<Guid> icdRoots);
}