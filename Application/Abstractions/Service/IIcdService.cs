using Application.Dto;

namespace Application.Abstractions.Service;

public interface IIcdService
{
    Task<List<Icd10RecordDto>> GetRootElements(); 
    Task<Icd10SearchDto> GetByNameAndParams(string name, int page, int size);
}