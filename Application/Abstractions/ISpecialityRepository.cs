using Domain;

namespace Application.Abstractions;

public interface ISpecialityRepository
{
    Task CreateAsync(Speciality speciality);
    Task<Speciality?> GetByIdAsync(Guid id);
    Task<IEnumerable<Speciality>> GetByNameAsync(string name, int page = 1, int size = 5);
}