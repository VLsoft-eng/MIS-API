using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class ConsultationService(
    IConsultationRepository consultationRepository,
    ISpecialityMapper specialityMapper,
    IConsultationMapper consultationMapper,
    ICommentService commentService)
    : IConsultationService
{
    public async Task<ConsultationDto> GetConsultation(Guid consultationId)
    {
        var consultation = await consultationRepository.GetById(consultationId);
        if (consultation == null)
        {
            throw new ConsultationNotFoundException();
        }

        var commentDtos = await commentService.GetCommentsByConsultationId(consultationId);
        var specialityDto = specialityMapper.ToDto(consultation.speciality);
        var consultationDto = consultationMapper.ToDto(consultation, specialityDto, commentDtos);
        
        return consultationDto;
    }
}