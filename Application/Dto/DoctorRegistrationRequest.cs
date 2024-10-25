using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dto;

public record DoctorRegistrationRequest(
    string name,
    
    string password,
    
    string email,
    
    DateTime birthday,
    
    Gender gender,
    
    string phone,
    
    Guid speciality
);
