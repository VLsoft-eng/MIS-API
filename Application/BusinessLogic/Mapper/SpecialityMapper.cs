using Application.Abstractions.Mapper;
using Application.Dto;
using Domain;

namespace Application.BusinessLogic.Mapper;

public class SpecialityMapper : ISpecialityMapper
{
    public List<SpecialityDto> toDto(List<Speciality> specialities)
    {
        var specialitiesDtos = specialities
            .Select(s => new SpecialityDto(
                s.id,
                s.CreateTime,
                s.name))
            .ToList();

        return specialitiesDtos;
    }
}