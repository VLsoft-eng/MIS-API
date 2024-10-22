using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Api.Dto;

public record UserRegistrationRequest(
    [property: Required]
    string Name,

    [property: Required]
    string Password,

    [property: Required, EmailAddress(ErrorMessage = "Invalid email format")]
    string Email,

    [property: Required]
    DateTime Birthday,

    [property: Required]
    Gender Gender,

    [property: Required, RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
    string Phone,

    [property: Required]
    Guid Speciality
);
