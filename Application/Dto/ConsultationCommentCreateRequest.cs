namespace Application.Dto;

public record ConsultationCommentCreateRequest(
    string content,
    Guid parentId);