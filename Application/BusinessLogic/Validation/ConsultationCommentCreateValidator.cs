using Application.Dto;
using FluentValidation;

namespace Application.BusinessLogic.Validation;

public class ConsultationCommentCreateValidator : AbstractValidator<ConsultationCommentCreateRequest>
{
    public ConsultationCommentCreateValidator()
    {
        RuleFor(c => c.content)
            .Length(1, 1000).WithMessage("Content must have length between 1 and 1000.");
    }
}