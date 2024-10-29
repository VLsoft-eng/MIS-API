using Application.Dto;
using Domain;

namespace Application.Abstractions.Mapper;

public interface IConsultationMapper
{
    Consultation ToEntity(Speciality speciality, Inspection inspection);
    ConsultationDto ToDto(Consultation consultation, SpecialityDto speciality, List<CommentDto> comments);

    InspectionConsultationDto ToInspectionConsultationDto(
        Consultation consultation,
        SpecialityDto speciality,
        InspectionCommentDto rootComment,
        Inspection inspection,
        int commentNumber);
}