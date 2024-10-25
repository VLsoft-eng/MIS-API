using Application.Dto;
using Domain;

namespace Application.Abstractions.Mapper;

public interface IDoctorMapper
{
    DoctorDto ToDto(Doctor doctor);
    Task<Doctor> ToEntity(DoctorRegistrationRequest request);
    void UpdateDoctorEntity(Doctor doctor, DoctorEditRequest request);
}