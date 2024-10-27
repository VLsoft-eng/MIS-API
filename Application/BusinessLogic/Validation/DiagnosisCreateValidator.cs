using Application.Dto;
using FluentValidation;

namespace Application.BusinessLogic.Validation;

public class DiagnosisCreateValidator : AbstractValidator<DiagnosisCreateRequest>
{
    public DiagnosisCreateValidator()
    {
        RuleFor(d => d.description)
            .Length(1, 5000).WithMessage("Description must have length between 1 and 5000.");
        RuleFor(d => d.type)
            .NotNull().WithMessage("Diagnosis type is required.")
            .IsInEnum().WithMessage("Invalid diagnosis type value.");
        RuleFor(d => d.icdDiagnosisId)
            .NotNull().WithMessage("Icd diagnosis id is required.");
        
    }
    
}