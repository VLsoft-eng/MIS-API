using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;

namespace Application.BusinessLogic.Service;

public class IcdService(IIcdRepository icdRepository, IIcdMapper icdMapper) : IIcdService
{
    public async Task<List<Icd10RecordDto>> GetRootElements()
    {
        var icds = await icdRepository.GetRootElements();
        var icdDtos = icdMapper.ToDto(icds);

        return icdDtos;
    }

    public async Task<Icd10SearchDto> GetByNameAndParams(string? name, int page, int size)
    {
        if (size <= 0 || page <= 0)
        {
            throw new InvalidPaginationParamsException();
        }

        var icds = await icdRepository.GetAllIcds();
        if (name != null)
        {
            icds = icds.Where(i => i.сode.ToLower().Contains(name.ToLower()) ||
                                   i.name.ToLower().Contains(name.ToLower()))
                .ToList();
        }
        
        var overAllIcds = icds.Count;
        var totalPages = (overAllIcds + size - 1) / size;
        
        if (page > totalPages)
        {
            throw new InvalidPaginationParamsException("Page must be smaller or equal page count.");
        }
        
        var pagedIcds = icds
            .Skip((page - 1) * size)
            .Take(size)
            .ToList();
        
        var icdsDtos = icdMapper.ToDto(pagedIcds);
        var pageInfoDto = new PageInfoDto(size, totalPages, page);

        return new Icd10SearchDto(icdsDtos, pageInfoDto);
    }
}