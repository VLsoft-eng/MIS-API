namespace Application.Dto;

public record CommentDto(
    Guid id,
    DateTime createTime,
    DateTime? modifiedDate,
    string content,
    Guid authorId,
    string author,
    Guid? parentId);