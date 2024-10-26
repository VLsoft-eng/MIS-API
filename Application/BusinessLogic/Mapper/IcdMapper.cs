using Application.Abstractions.Mapper;
using Application.Dto;
using Domain;

namespace Application.BusinessLogic.Mapper;

public class IcdMapper : IIcdMapper
{
    public List<Icd10RecordDto> ToDto(List<Icd> icds)
    {
        var icdDtos = icds
            .Select(icd => new Icd10RecordDto(
                icd.id,
                icd.createTime,
                icd.сode,
                icd.name))
            .ToList();

        return icdDtos;
    }
}