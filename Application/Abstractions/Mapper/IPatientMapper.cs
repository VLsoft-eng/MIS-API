using Application.Dto;
using Domain;

namespace Application.Abstractions.Mapper;

public interface IPatientMapper
{
    Patient ToEntity(PatientCreateRequest request);
    PatientDto ToDto(Patient entity);
}