namespace Application.Dto;

public record InspectionCommentDto(
    Guid id,
    DateTime createTime,
    Guid? parentId,
    string? content,
    DoctorDto author,
    DateTime? modifyTime);