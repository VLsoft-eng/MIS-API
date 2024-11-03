using Application.BusinessLogic.Enums;
using Application.Dto;
using Domain.Enums;

namespace Application.Abstractions.Service;

public interface IPatientService
{
    Task<PatientDto> GetPatientById(Guid id);
    Task<Guid> Create(PatientCreateRequest request);
    Task<Guid> CreatePatientsInspection(Guid patientId, Guid doctorId, InspectionCreateRequest request);
    Task<List<InspectionShortDto>> SearchPatientInspectionsByParams(Guid patientId, string request);

    Task<InspectionPagedListDto> GetPatientInspectionsByParams(
        Guid patientId,
        bool grouped,
        List<Guid> icdRoots,
        int page,
        int size);

    Task<PatientPagedListDto> GetPatientsByParams(
        string? request,
        List<Conclusion>? conclusions,
        SortingType? sorting,
        bool scheduledVisits,
        bool onlyMine,
        int page,
        int size,
        Guid doctorId);
}