using FleetOps.Application.Validations;
using FluentValidation;

namespace FleetOps.Application.Drivers.CreateDriver;

public sealed class CreateDriverCommandValidator : AbstractValidator<CreateDriverCommand>
{
    public CreateDriverCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ValidationErrorCodes.Driver.Name.Required)
            .WithMessage(ValidationConstants.MessageTemplate.Required)
            .MaxNameLength(ValidationErrorCodes.Driver.Name.MaxLengthExceeded);
    }
}