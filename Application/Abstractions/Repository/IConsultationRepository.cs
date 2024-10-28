using Domain;

namespace Application.Abstractions.Repository;

public interface IConsultationRepository
{
    Task Create(Consultation consultation);
    Task<Consultation?> GetById(Guid consultationId);
}