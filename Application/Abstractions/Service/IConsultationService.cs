using Application.Dto;

namespace Application.Abstractions.Service;

public interface IConsultationService
{
    Task<ConsultationDto> GetConsultation(Guid consultationId);
}