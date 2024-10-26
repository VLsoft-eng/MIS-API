using Application.Dto;

namespace Application.Abstractions.Service;

public interface IPatientService
{
    Task<PatientDto> GetPatientById(Guid id);
    Task Create(PatientCreateRequest request);

}