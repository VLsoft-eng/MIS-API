using Application.Dto;
using Domain.Enums;
using FluentValidation;

namespace Application.BusinessLogic.Validation;

public class InspectionCreateValidator : AbstractValidator<InspectionCreateRequest>
{
    public InspectionCreateValidator()
    {
        RuleFor(ins => ins.date)
            .NotNull().WithMessage("Date is required.");
        RuleFor(ins => ins.anamnesis)
            .NotNull().WithMessage("Anamnesis is required.")
            .Length(1, 5000).WithMessage("Anamnesis must have length between 1 and 5000.");
        RuleFor(ins => ins.complaints)
            .NotNull().WithMessage("Complaints is required")
            .Length(1, 5000).WithMessage("Complaints must have length between 1 and 5000.");
        RuleFor(ins => ins.conclusion)
            .NotNull().WithMessage("Conclusion is required.")
            .IsInEnum().WithMessage("Invalid conclusion value.");
        RuleFor(ins => ins.nextVisitDate) 
            .Must(date => date == null)
            .When(ins => ins.conclusion  == Conclusion.Death)
            .WithMessage("The date of the next visit cannot be set if the patient dies.");
        RuleFor(ins => ins.deathDate)
            .Must(date => date == null)
            .When(ins => ins.conclusion != Conclusion.Death)
            .WithMessage("The date of death cannot be set unless the patient is deceased.");
        RuleFor(x => x.diagnoses)
            .NotNull()
            .WithMessage("Diagnoses are required.")
            .NotEmpty()
            .WithMessage("Must be at least one diagnosis.");
        RuleFor(x => x.diagnoses)
            .Must(diagnoses => diagnoses.Count(d => d.type == DiagnosisType.Main) == 1)
            .WithMessage("There must be one and only one diagnosis with the Main type.");
    }
}