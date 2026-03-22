using FluentValidation;

namespace FleetOps.Application.Validations;

public static class StringValidationExtensions
{
    public static IRuleBuilderOptions<T, string?> MaxNameLength<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder
            .MaximumLength(ValidationConstants.Names.MaxLength)
            .WithMessage("{PropertyName} is {TotalLength}, but must be at most {MaxLength} characters long.");
}