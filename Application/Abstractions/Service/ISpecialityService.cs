using Application.Dto;

namespace Application.Abstractions.Service;

public interface ISpecialityService
{
    Task<SpecialitiesPagedListDto> GetByNameAndParams(string name, int page, int size);
}