using Application.Dto;
using FluentValidation;

namespace Application.BusinessLogic.Validation;

public class DoctorLoginValidator : AbstractValidator<DoctorLoginRequest>
{
    DoctorLoginValidator()
    {
        RuleFor(doctor => doctor.email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(doctor => doctor.password)
            .NotEmpty().WithMessage("Password is required");
    }
    
}