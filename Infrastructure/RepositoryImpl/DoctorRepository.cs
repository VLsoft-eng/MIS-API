using Application.Abstractions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.RepositoryImpl;

public class DoctorRepository : IDoctorRepository
{
    private ApplicationDbContext _context;

    public DoctorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Create(Doctor doctor)
    {
        _context.Doctors.Add(doctor);
    }

    public void Update(Doctor doctor)
    {
        _context.Entry(doctor).State = EntityState.Modified;
    }

    public Doctor? GetById(Guid id)
    {
        return _context.Doctors.Find(id);
    }

}