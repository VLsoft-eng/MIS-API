using Domain.Enums;

namespace Application.Dto;

public record DiagnosisCreateRequest(
    Guid icdDiagnosisId,
    string? description,
    DiagnosisType type);