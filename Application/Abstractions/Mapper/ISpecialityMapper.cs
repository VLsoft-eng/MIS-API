using Application.Dto;
using Domain;

namespace Application.Abstractions.Mapper;

public interface ISpecialityMapper
{
    List<SpecialityDto> ToDto(List<Speciality> specialities);
    SpecialityDto ToDto(Speciality speciality);
}