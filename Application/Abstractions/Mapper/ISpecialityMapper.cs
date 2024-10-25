using Application.Dto;
using Domain;

namespace Application.Abstractions.Mapper;

public interface ISpecialityMapper
{
    List<SpecialityDto> toDto(List<Speciality> specialities);
}