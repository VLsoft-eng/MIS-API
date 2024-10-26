using Domain.Enums;

namespace Application.Dto;

public record PatientCreateRequest(
    string name,
    DateTime birthday,
    Gender gender);