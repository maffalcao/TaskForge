using Domain.Dtos;
using FluentValidation;

namespace Api.Validators;

public class AddCommentDtoValidator : AbstractValidator<AddTaskCommentDto>
{
    public AddCommentDtoValidator()
    {
        RuleFor(x => x.Comment)
            .NotEmpty()
            .WithMessage("Comment_CantBeEmpty");
    }
}
