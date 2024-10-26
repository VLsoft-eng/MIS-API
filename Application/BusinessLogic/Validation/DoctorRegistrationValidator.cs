using FluentValidation;
using Application.Dto;

namespace Application.BusinessLogic.Validation;

public class DoctorRegistrationValidator : AbstractValidator<DoctorRegistrationRequest>
{
    public DoctorRegistrationValidator()
    {
        RuleFor(doctor => doctor.name)
            .NotEmpty().WithMessage("Name is required");
        RuleFor(doctor => doctor.password)
            .NotEmpty().WithMessage("Password is required");
        RuleFor(doctor => doctor.email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
        RuleFor(doctor => doctor.birthday)
            .NotEmpty().WithMessage("Birthday is required")
            .LessThan(DateTime.UtcNow).WithMessage("Birthday must be in the past");
        RuleFor(doctor => doctor.gender)
            .IsInEnum().WithMessage("Invalid gender value");
        RuleFor(doctor => doctor.phone)
            .NotEmpty().WithMessage("Phone is required")
            .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Phone number is not valid");
        RuleFor(doctor => doctor.speciality)
            .NotEqual(Guid.Empty).WithMessage("Speciality is required");
    }
}