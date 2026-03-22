using FluentValidation;

namespace FleetOps.Application.Validations;

public static class PaginationValidationExtensions
{
    public static IRuleBuilderOptions<T, int> ValidLimit<T>(this IRuleBuilder<T, int> ruleBuilder)
        => ruleBuilder
            .InclusiveBetween(ValidationConstants.Pagination.MinPageSize, ValidationConstants.Pagination.MaxPageSize)
            .WithMessage("{PropertyName} must be between {From} and {To}.");

    public static IRuleBuilderOptions<T, int> ValidOffset<T>(this IRuleBuilder<T, int> ruleBuilder) 
        => ruleBuilder
            .GreaterThanOrEqualTo(0)
            .WithMessage("{PropertyName} must be greater than or equal to {ComparisonValue}.");
}