using Application.Dto;
using Domain;
using Domain.Enums;
using FluentValidation;

namespace Application.BusinessLogic.Validation;

public class InspectionEditValidator : AbstractValidator<InspectionEditRequest>
{
    public InspectionEditValidator()
    {
        RuleFor(ins => ins.anamnesis)
            .NotNull().WithMessage("Anamnesis is required.")
            .Length(1, 5000).WithMessage("Anamnesis must have length between 1 and 5000.");
        RuleFor(ins => ins.complaints)
            .NotNull().WithMessage("Complaints is required")
            .Length(1, 5000).WithMessage("Complaints must have length between 1 and 5000.");
        RuleFor(i => i.treatment)
            .NotNull().WithMessage("Treatment is required")
            .Length(1, 5000).WithMessage("Complaints must have length between 1 and 5000");
        RuleFor(i => i.conclusion)
            .NotNull().WithMessage("Conclusion is required")
            .IsInEnum().WithMessage("Invalid conclusion value.");
        RuleFor(ins => ins.nextVisitDate) 
            .Must(date => date == null)
            .When(i => i.conclusion  != Conclusion.Disease)
            .WithMessage("The date of the next visit cannot be set if conclusion is not disease.");
        RuleFor(ins => ins.nextVisitDate) 
            .Must(date => date != null)
            .When(i => i.conclusion  == Conclusion.Disease)
            .WithMessage("The date of the next visit is required if conclusion is disease.");
        RuleFor(ins => ins.deathDate)
            .Must(date => date == null)
            .When(ins => ins.conclusion != Conclusion.Death)
            .WithMessage("Death date cannot be set if conclusion is not death.");
        RuleFor(ins => ins.deathDate)
            .Must(date => date != null)
            .When(ins => ins.conclusion == Conclusion.Death)
            .WithMessage("Death date is required if conclusion is death.");
        RuleFor(x => x.diagnoses)
            .Must(diagnoses => diagnoses.Count(d => d.type == DiagnosisType.Main) == 1)
            .WithMessage("There must be one and only one diagnosis with the Main type.");
    }
}