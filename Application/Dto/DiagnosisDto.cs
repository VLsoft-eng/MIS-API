using Domain.Enums;

namespace Application.Dto;

public record DiagnosisDto(
    Guid id,
    DateTime createTime,
    string code,
    string name,
    string decription,
    DiagnosisType type);