namespace Application.Dto;

public record InspectionShortDto(
    DateTime date,
    DiagnosisDto diagnosis,
    Guid id,
    DateTime createTime
    );