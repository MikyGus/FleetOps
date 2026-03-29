using FleetOps.Application.Validations;
using FluentValidation;

namespace FleetOps.Application.Drivers.CreateDriver;

public sealed class CreateDriverCommandValidator : AbstractValidator<CreateDriverCommand>
{
    public CreateDriverCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(ValidationConstants.MessageTemplateRequired)
            .MaxNameLength();
    }
}