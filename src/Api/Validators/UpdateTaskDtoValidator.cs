using Domain.Dtos;
using FluentValidation;

namespace Api.Validators;

public class UpdateTaskDtoValidator: AbstractValidator<UpdateTaskDto>
{    public UpdateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title_CantBeEmpty");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description_CantBeEmpty");

        RuleFor(x => x.DueDate)
            .NotEmpty()
            .When(x => x.DueDate.HasValue)
                .Must(BeAFutureDate)
                .When(x => x.DueDate.HasValue)
                .WithMessage("DueDate_CantBeInThePast");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status_IsInvalid");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .WithMessage("Priority_IsInvalid");
    }

    private bool BeAFutureDate(DateTime? dueDate)
    {
        return dueDate > DateTime.Now;
    }
}
