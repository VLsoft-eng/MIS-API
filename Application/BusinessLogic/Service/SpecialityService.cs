using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Dto;
using Application.Abstractions.Service;

namespace Application.BusinessLogic.Service;

public class SpecialityService : ISpecialityService
{
    private readonly ISpecialityRepository _specialityRepository;
    private readonly ISpecialityMapper _specialityMapper;

    public SpecialityService(ISpecialityRepository specialityRepository, ISpecialityMapper specialityMapper)
    {
        _specialityRepository = specialityRepository;
        _specialityMapper = specialityMapper;
    }

    public async Task<SpecialitiesPagedListDto> GetByNameAndParams(string name, int page, int size)
    {
        var specialities = await _specialityRepository.GetByNameAndParams(name, page, size);
        var overAllSpecialitiesWithName = await _specialityRepository.GetCountByName(name);
        var totalPages = (int)Math.Ceiling((double)overAllSpecialitiesWithName / size);

        var specialitiesDtos = _specialityMapper.toDto(specialities);
        var pageInfoDto = new PageInfoDto(size, totalPages, page);

        return  new SpecialitiesPagedListDto(specialitiesDtos, pageInfoDto);
    }
}