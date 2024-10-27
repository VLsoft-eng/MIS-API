using Application.Dto;
using FluentValidation;

namespace Application.BusinessLogic.Validation;

public class InspectionCommentCreateValidator : AbstractValidator<InspectionCommentCreateRequest>
{
    public InspectionCommentCreateValidator()
    {
        RuleFor(c => c.content)
            .NotNull().WithMessage("Comment content is required")
            .Length(1, 1000).WithMessage("Content length must have length between 1 and 1000.");
    }
}