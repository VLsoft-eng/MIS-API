using Domain;

namespace Application.Abstractions.Repository;

public interface IIcdRepository
{
    Task<List<Icd>> GetRootElements();
    Task<List<Icd>> GetByNameAndParams(string name, int page, int size);
    Task<int> GetCountByName(string name);
    Task<Icd?> GetById(Guid id);
    Task<Icd> GetRootByIcdId(Guid id);
}