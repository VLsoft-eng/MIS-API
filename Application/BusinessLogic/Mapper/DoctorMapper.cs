using Application.Dto;
using Application.Abstractions;
using Domain;

namespace Application.BusinessLogic.Mapper;

public class DoctorMapper
{
    private readonly ISpecialityRepository _specialityRepository;

    public DoctorMapper(ISpecialityRepository specialityRepository)
    {
        _specialityRepository = specialityRepository;
    }
    
    public DoctorDto toDto(Doctor doctor)
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
        var speciality = await _specialityRepository.GetById(request.speciality);
        
        if (speciality == null)
        {
            throw new Exception();
        }

        return new Doctor
        {
            id = Guid.NewGuid(),
            createTime = DateTime.UtcNow,
            name = request.name,
            birthday = request.birthday,
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