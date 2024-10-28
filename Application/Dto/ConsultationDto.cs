namespace Application.Dto;

public record ConsultationDto(
    Guid id,
    DateTime createTime,
    Guid inspectionId,
    SpecialityDto speciality,
    List<CommentDto> comments);