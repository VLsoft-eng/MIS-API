using Application.Abstractions.Mapper;
using Application.Dto;
using Domain;

namespace Application.BusinessLogic.Mapper;

public class PatientMapper : IPatientMapper
{
    public Patient ToEntity(PatientCreateRequest request)
    {
        return new Patient
        {
            id = Guid.NewGuid(),
            name = request.name,
            birtday = request.birthday,
            createTime = DateTime.UtcNow,
            gender = request.gender
        };
    }

    public PatientDto ToDto(Patient entity)
    {
        return new PatientDto(
            entity.id,
            entity.createTime,
            entity.name,
            entity.birtday,
            entity.gender);
    }
}