using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Api.Dto;

public record UserRegistrationRequest
{
    [Required]
    public string name { get; init; }
    
    [Required]
    public string password { get; init; }
    
    [Required, EmailAddress(ErrorMessage = "Invalid email format")]
    public string email { get; init; }
    
    [Required]
    public DateTime Birthday { get; init; }
    
    [Required]
    public Gender Gender { get; init; }
    
    [Required, RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
    public string Phone { get; init; }
    
    [Required]
    public Guid Speciality { get; init; }
};