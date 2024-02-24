using Domain.Dtos;
using FluentValidation;
namespace Api.Validators;



public class AddProjectDtoValidator : AbstractValidator<AddProjectDto>
{
    public AddProjectDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name_CantBeEmpty");
    }
}
