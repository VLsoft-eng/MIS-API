using Domain;

namespace Application.Abstractions.Repository;

public interface IPatientRepository
{
    Task Create(Patient patient);
    Task<Patient?> GetById(Guid id);
    Task<List<Patient>> GetAllPatients();
}