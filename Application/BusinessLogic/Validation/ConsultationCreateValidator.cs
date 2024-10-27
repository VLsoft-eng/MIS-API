using Application.Dto;
using FluentValidation;

namespace Application.BusinessLogic.Validation;

public class ConsultationCreateValidator : AbstractValidator<ConsultationCreateRequest>
{
    public ConsultationCreateValidator()
    {
        RuleFor(c => c.specialityId)
            .NotNull().WithMessage("Speciality id is required.");

        RuleFor(c => c.comment)
            .NotNull().WithMessage("Consultation root comment is required.");
    }
}