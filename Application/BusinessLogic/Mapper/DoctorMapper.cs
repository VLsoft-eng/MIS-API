using Application.Abstractions.Mapper;
using Application.Dto;
using Application.Abstractions.Repository;
using Domain;
using Application.Exceptions;

namespace Application.BusinessLogic.Mapper;

public class DoctorMapper(ISpecialityRepository specialityRepository) : IDoctorMapper
{
    public DoctorDto ToDto(Doctor doctor)
    { 
        return new DoctorDto(
            doctor.id,
            doctor.createTime,
            doctor.name,
            doctor.birthday,
            doctor.gender,
            doctor.email,
            doctor.phone
        );
    }

    public async Task<Doctor> ToEntity(DoctorRegistrationRequest request)
    {
        var speciality = await specialityRepository.GetById(request.speciality);
        
        if (speciality == null)
        {
            throw new SpecialityNotFoundException();
        }

        return new Doctor
        {
            id = Guid.NewGuid(),
            createTime = DateTime.UtcNow,
            name = request.name,
            birthday = request.birthday,
            hashedPassword = request.password,
            gender = request.gender,
            email = request.email,
            phone = request.phone,
            speciality = speciality
        };
    }
    
    public void UpdateDoctorEntity(Doctor doctor, DoctorEditRequest request)
    {
        doctor.email = request.email;
        doctor.name = request.name;
        doctor.birthday = request.birthday;
        doctor.gender = request.gender;
        doctor.phone = request.phone;
    }
}