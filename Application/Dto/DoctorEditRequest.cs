using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dto;

public record DoctorEditRequest(
    string Name,
    
    string Password,
    
    string Email,
    
    DateTime Birthday,
    
    Gender Gender,
    
    string Phone,
    
    Guid Speciality
    );