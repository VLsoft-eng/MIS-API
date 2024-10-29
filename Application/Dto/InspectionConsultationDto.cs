namespace Application.Dto;

public record InspectionConsultationDto(
    Guid? id,
    DateTime createTime,
    Guid? inspectionId,
    SpecialityDto speciality,
    InspectionCommentDto rootComment,
    int commentsNumber);