using FluentValidation;

namespace FleetOps.Application.Validations;

public static class IdentifierValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid> ValidRequiredId<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        => ruleBuilder
            .NotEmpty()
            .WithMessage("{PropertyName} must not be a non-empty GUID.");
}