using Domain;

namespace Application.Abstractions.Repository;

public interface IDoctorRepository
{
    public Task Create(Doctor doctor);
    public Task Update(Doctor doctor);
    public Task <Doctor?>GetById(Guid id);
    public Task<Doctor?> GetByEmail(string email);
}