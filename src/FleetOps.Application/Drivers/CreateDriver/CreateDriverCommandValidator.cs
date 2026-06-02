using FleetOps.Application.Validations;
using FleetOps.Domain.Errors;
using FluentValidation;

namespace FleetOps.Application.Drivers.CreateDriver;

public sealed class CreateDriverCommandValidator : AbstractValidator<CreateDriverCommand>
{
    public CreateDriverCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithErrorCode(ErrorCodes.Driver.Name.Required)
            .WithMessage(ValidationConstants.MessageTemplate.Required)
            .MaxNameLength(ErrorCodes.Driver.Name.MaxLengthExceeded);
    }
}
