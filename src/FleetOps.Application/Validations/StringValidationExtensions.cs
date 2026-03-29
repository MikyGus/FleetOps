using FluentValidation;

namespace FleetOps.Application.Validations;

public static class StringValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> MaxNameLength<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder
            .MaximumLength(ValidationConstants.Names.MaxLength)
                .WithMessage("{PropertyName} is {TotalLength}, but must be at most {MaxLength} characters long.");

    public static IRuleBuilderOptions<T, string> ValidRegistrationNumber<T>(this IRuleBuilder<T, string> ruleBuilder) 
        => ruleBuilder
            .NotEmpty()
                .WithMessage("{PropertyName} may not be empty.")
            .MaximumLength(ValidationConstants.RegistrationNumber.MaxLength)
                .WithMessage(ValidationConstants.RegistrationNumber.MessageTemplate);
    
    public static IRuleBuilderOptions<T, string?> ValidRegistrationNumberOptional<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder
            .MaximumLength(ValidationConstants.RegistrationNumber.MaxLength)
                .WithMessage(ValidationConstants.RegistrationNumber.MessageTemplate);

}