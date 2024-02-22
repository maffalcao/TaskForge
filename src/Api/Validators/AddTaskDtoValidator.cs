using Domain.Dtos;
using Domain.Entities;
using FluentValidation;
using System;

public class AddTaskDtoValidator : AbstractValidator<AddTaskDto>
{
    public AddTaskDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.DueDate).NotEmpty().When(x => x.DueDate.HasValue).WithMessage("A data de vencimento precisa ser uma data no futuro.")
                               .Must(BeAFutureDate).When(x => x.DueDate.HasValue).WithMessage("A data de vencimento deve estar no futuro.");
        RuleFor(x => x.Status).IsInEnum().WithMessage("O status fornecido não é válido.");
        RuleFor(x => x.Priority).IsInEnum().WithMessage("A prioridade fornecida não é válida.");
    }

    private bool BeAFutureDate(DateTime? dueDate)
    {
        return dueDate > DateTime.Now;
    }
}
