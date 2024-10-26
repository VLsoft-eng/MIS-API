using Application.Dto;
using Domain;

namespace Application.Abstractions.Mapper;

public interface IIcdMapper
{
    List<Icd10RecordDto> ToDto(List<Icd> icds);
}