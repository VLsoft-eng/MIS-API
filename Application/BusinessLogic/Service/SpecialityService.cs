using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Dto;
using Application.Abstractions.Service;
using Application.Exceptions;

namespace Application.BusinessLogic.Service;

public class SpecialityService(ISpecialityRepository specialityRepository, ISpecialityMapper specialityMapper)
    : ISpecialityService
{
    public async Task<SpecialitiesPagedListDto> GetByNameAndParams(string? name, int page, int size)
    {
        if (size <= 0 || page <= 0)
        {
            throw new InvalidPaginationParamsException();
        }

        var specialities = await specialityRepository.GetAllSpecialities();

        if (name != null)
        {
            specialities = specialities.Where(s => s.name.ToLower().Contains(name.ToLower())).ToList();
        }

        var overAllSpecialities = specialities.Count;
        var totalPages = (int)Math.Ceiling((double)overAllSpecialities / size);
        
        if (page > totalPages)
        {
            throw new InvalidPaginationParamsException("Page must be smaller or equal page count.");
        }
        
        var specialitiesDtos = specialityMapper.ToDto(specialities);
        var pageInfoDto = new PageInfoDto(size, totalPages, page);

        return new SpecialitiesPagedListDto(specialitiesDtos, pageInfoDto);
    }
}