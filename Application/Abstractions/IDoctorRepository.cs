using Domain;

namespace Application.Abstractions;

public interface IDoctorRepository
{
    public void Create(Doctor doctor);
    public void Update(Doctor doctor);
    public Doctor? GetById(Guid id);
}