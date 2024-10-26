using Domain.Enums;

namespace Application.Dto;

public record PatientDto(
    Guid id,
    DateTime createTime,
    string name,
    DateTime birtday,
    Gender gender);