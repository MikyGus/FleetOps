using FleetOps.Domain.Errors;
using FluentValidation;

namespace FleetOps.Application.Validations;

public static class StringValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> MaxNameLength<T>(
        this IRuleBuilder<T, string?> ruleBuilder,
        string errorCode)
        => ruleBuilder
            .MaximumLength(ValidationConstants.Names.MaxLength)
                .WithErrorCode(errorCode)
                .WithMessage("{PropertyName} is {TotalLength}, but must be at most {MaxLength} characters long.");

    public static IRuleBuilderOptions<T, string> ValidRegistrationNumber<T>(
        this IRuleBuilder<T, string> ruleBuilder) 
        => ruleBuilder
            .NotEmpty()
                .WithMessage(ValidationConstants.MessageTemplate.Required)
                .WithErrorCode(ErrorCodes.Vehicle.RegistrationNumber.Required)
            .MaximumLength(ValidationConstants.RegistrationNumber.MaxLength)
                .WithMessage(ValidationConstants.RegistrationNumber.MessageTemplate)
                .WithErrorCode(ErrorCodes.Vehicle.RegistrationNumber.MaxLengthExceeded);
    
    public static IRuleBuilderOptions<T, string?> ValidRegistrationNumberOptional<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder
            .MaximumLength(ValidationConstants.RegistrationNumber.MaxLength)
                .WithMessage(ValidationConstants.RegistrationNumber.MessageTemplate)
                .WithErrorCode(ErrorCodes.Vehicle.RegistrationNumber.MaxLengthExceeded);

}