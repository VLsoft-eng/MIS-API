using Domain.Enums;

namespace Application.Dto;

public record DoctorDto(
    Guid id,
    DateTime createTime,
    string name,
    DateTime birthday,
    Gender gender,
    string email,
    string phone
    );