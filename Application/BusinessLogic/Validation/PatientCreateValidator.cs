using Application.Dto;
using FluentValidation;

namespace Application.BusinessLogic.Validation;

public class PatientCreateValidator : AbstractValidator<PatientCreateRequest>
{
    public PatientCreateValidator()
    {
        RuleFor(patient => patient.name)
            .NotEmpty().WithMessage("Name is required");
        RuleFor(patient => patient.birthday)
            .NotEmpty().WithMessage("Birthday date is required")
            .LessThan(DateTime.UtcNow).WithMessage("Birthday must be in the past");
        RuleFor(patient => patient.gender)
            .NotNull().WithMessage("Gender is required")
            .IsInEnum().WithMessage("Invalid gender value");
    }
}