using Application.Dto;
using FluentValidation;

namespace Application.BusinessLogic.Validation;

public class CommentEditValidator : AbstractValidator<CommentEditRequest>
{
    public CommentEditValidator()
    {
        RuleFor(comment => comment.content)
            .NotNull().WithMessage("Comment content is required.")
            .Length(1, 1000).WithMessage("Comment must have length between 1 and 1000.");
    }
}