namespace Application.Dto;

public record ConsultationCreateRequest(
    Guid specialityId,
    InspectionCommentCreateRequest comment);