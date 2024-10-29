using Application.Dto;
using Domain;

namespace Application.Abstractions.Repository;

public interface IConsultationRepository
{
    Task Create(Consultation consultation);
    Task<Consultation?> GetById(Guid consultationId);
    Task<List<Consultation>> GetBySpecialityId(Guid id);
    Task<List<Consultation>> GetByInspectionId(Guid id);
}