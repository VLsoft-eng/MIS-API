using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.Dto;

public record DoctorEditRequest(
    string email,
    
    string name,
    
    DateTime birthday,
    
    Gender gender,
    
    string phone
    );