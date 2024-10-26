using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;

namespace Application.BusinessLogic.Service;

public class IcdService : IIcdService
{
    private readonly IIcdRepository _icdRepository;
    private readonly IIcdMapper _icdMapper;

    public IcdService(IIcdRepository icdRepository, IIcdMapper icdMapper)
    {
        _icdRepository = icdRepository;
        _icdMapper = icdMapper;
    }

    public async Task<List<Icd10RecordDto>> GetRootElements()
    {
        var icds = await _icdRepository.GetRootElements();
        var icdDtos = _icdMapper.ToDto(icds);

        return icdDtos;
    }

    public async Task<Icd10SearchDto> GetByNameAndParams(string name, int page, int size)
    {
        var icds = await _icdRepository.GetByNameAndParams(name, page, size);
        var overAllIcdsWithName = await _icdRepository.GetCountByName(name);
        var totalPages = (int)Math.Ceiling((double)overAllIcdsWithName / size);

        var specialitiesDtos = _icdMapper.ToDto(icds);
        var pageInfoDto = new PageInfoDto(size, totalPages, page);

        return new Icd10SearchDto(specialitiesDtos, pageInfoDto);
    }
}