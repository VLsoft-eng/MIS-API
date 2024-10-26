namespace Application.Dto;

public record Icd10RecordDto(
    Guid id,
    DateTime createTime,
    string code,
    string name);