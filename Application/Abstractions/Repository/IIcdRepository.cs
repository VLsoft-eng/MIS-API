using Domain;

namespace Application.Abstractions.Repository;

public interface IIcdRepository
{
    Task<List<Icd>> GetRootElements();
    Task<List<Icd>> GetAllIcds();
    Task<Icd?> GetById(Guid id);
    Task<Icd> GetRootByIcdId(Guid id);
}