using Domain;

namespace Application.Abstractions.Repository;

public interface ISpecialityRepository
{
    Task Create(Speciality speciality);
    Task<Speciality?> GetById(Guid id);
    Task<List<Speciality>> GetAllSpecialities();
}