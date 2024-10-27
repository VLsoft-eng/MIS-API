using Domain.Enums;

namespace Application.Dto;

public record DiagnosisDto(
    string code,
    string name,
    string decription,
    DiagnosisType type,
    Guid id,
    DateTime createTime);