using FluentValidation;

namespace FleetOps.Application.Drivers.CreateDriver;

public sealed class CreateDriverCommandValidator : AbstractValidator<CreateDriverCommand>
{
    public CreateDriverCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} may not be empty.")
            .MaximumLength(200)
            .WithMessage("{PropertyName} is {TotalLength}, but must be at most {MaxLength} characters long.");
    }
}