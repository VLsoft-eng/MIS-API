using Application.Abstractions.Mapper;
using Application.Dto;
using Domain;

namespace Application.BusinessLogic.Mapper;

public class ConsultationMapper : IConsultationMapper
{
    public Consultation ToEntity(Speciality speciality, Inspection inspection)
    {
        return new Consultation
        {
            id = Guid.NewGuid(),
            createTime = DateTime.UtcNow,
            inspection = inspection,
            speciality = speciality
        };
    }

    public ConsultationDto ToDto(Consultation consultation, SpecialityDto speciality, List<CommentDto> comments)
    {
        return new ConsultationDto(
            consultation.id,
            consultation.createTime,
            consultation.inspection.id,
            speciality,
            comments);
    }
}