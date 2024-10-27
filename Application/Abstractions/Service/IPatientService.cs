using Application.Dto;

namespace Application.Abstractions.Service;

public interface IPatientService
{
    Task<PatientDto> GetPatientById(Guid id);
    Task Create(PatientCreateRequest request);
    Task<Guid> CreatePatientsInspection(Guid patientId, Guid doctorId, InspectionCreateRequest request);
    Task<List<InspectionShortDto>> SearchPatientInspectionsByParams(Guid patientId, string request);

    Task<InspectionPagedListDto> GetPatientInspectionsByParams(
        Guid patientId,
        bool grouped,
        List<Guid> icdRoots,
        int page,
        int size);
}