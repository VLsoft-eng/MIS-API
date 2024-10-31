using Application.Abstractions.Mapper;
using Application.Abstractions.Repository;
using Application.Abstractions.Service;
using Application.Dto;
using Application.Exceptions;
using Domain;
using FluentValidation;

namespace Application.BusinessLogic.Service;

public class ConsultationService : IConsultationService
{
    private readonly IConsultationRepository _consultationRepository;
    private readonly ISpecialityMapper _specialityMapper;
    private readonly IConsultationMapper _consultationMapper;
    private readonly ICommentService _commentService;

    public ConsultationService(
        IConsultationRepository consultationRepository,
        ISpecialityMapper specialityMapper,
        IConsultationMapper consultationMapper,
        IIcdRepository icdRepository,
        ICommentService commentService)
    {
        _consultationRepository = consultationRepository;
        _specialityMapper = specialityMapper;
        _consultationMapper = consultationMapper;
        _commentService = commentService;
    }
    
    public async Task<ConsultationDto> GetConsultation(Guid consultationId)
    {
        var consultation = await _consultationRepository.GetById(consultationId);
        if (consultation == null)
        {
            throw new ConsultationNotFoundException();
        }

        var commentDtos = await _commentService.GetCommentsByConsultationId(consultationId);
        var specialityDto = _specialityMapper.ToDto(consultation.speciality);
        var consultationDto = _consultationMapper.ToDto(consultation, specialityDto, commentDtos);
        
        return consultationDto;
    }
}