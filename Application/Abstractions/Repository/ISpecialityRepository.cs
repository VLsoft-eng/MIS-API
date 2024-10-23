using Domain;

namespace Application.Abstractions.Repository;

public interface ISpecialityRepository
{
    Task Create(Speciality speciality);
    Task<Speciality?> GetById(Guid id);
    Task<IEnumerable<Speciality>> GetByNameAndParams(string name, int page = 1, int size = 5);
}