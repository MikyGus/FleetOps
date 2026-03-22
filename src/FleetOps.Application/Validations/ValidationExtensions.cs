using FluentValidation;
using System.Linq.Expressions;

namespace FleetOps.Application.Validations;

public static class ValidationExtentions
{
    public static IRuleBuilderOptions<T, int> ValidLimit<T>(this IRuleBuilder<T, int> ruleBuilder)
        => ruleBuilder
            .InclusiveBetween(ValidationConstants.Pagination.MinPageSize, ValidationConstants.Pagination.MaxPageSize)
            .WithMessage("{PropertyName} must be between {From} and {To}.");

    public static IRuleBuilderOptions<T, int> ValidOffset<T>(this IRuleBuilder<T, int> ruleBuilder) 
        => ruleBuilder
            .GreaterThanOrEqualTo(0)
            .WithMessage("{PropertyName} must be greater than or equal to {ComparisonValue}.");

    public static IRuleBuilderOptions<T, string?> MaxNameLength<T>(this IRuleBuilder<T, string?> ruleBuilder)
        => ruleBuilder
            .MaximumLength(ValidationConstants.Names.MaxLength)
            .WithMessage("{PropertyName} is {TotalLength}, but must be at most {MaxLength} characters long.");

    public static IRuleBuilderOptions<T, Guid> ValidRequiredId<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        => ruleBuilder
            .NotEmpty()
            .WithMessage("{PropertyName} must not be a non-empty GUID.");

    public static IRuleBuilderOptionsConditions<T, T> ValidDateOrder<T>(
        this IRuleBuilderInitial<T, T> ruleBuilder,
        Expression<Func<T, DateTimeOffset?>> fromExpression,
        Expression<Func<T, DateTimeOffset?>> toExpression)
    {
        Func<T, DateTimeOffset?> fromSelector = fromExpression.Compile();
        Func<T, DateTimeOffset?> toSelector = toExpression.Compile();

        string fromName = GetMemberName(fromExpression);
        string toName = GetMemberName(toExpression);

        return ruleBuilder.Custom((instance, context) =>
        {
            DateTimeOffset? from = fromSelector(instance);
            DateTimeOffset? to = toSelector(instance);

            if (from.HasValue && to.HasValue && from >= to)
            {
                context.AddFailure(fromName, $"{fromName} must be earlier than {toName}.");
                context.AddFailure(toName, $"{toName} must be later than {fromName}.");
            }
        });
    }

        public static IRuleBuilderOptionsConditions<T, T> ValidDateOrder<T>(
        this IRuleBuilderInitial<T, T> ruleBuilder,
        Expression<Func<T, DateTimeOffset>> fromExpression,
        Expression<Func<T, DateTimeOffset>> toExpression)
    {
        Func<T, DateTimeOffset> fromSelector = fromExpression.Compile();
        Func<T, DateTimeOffset> toSelector = toExpression.Compile();

        string fromName = GetMemberName(fromExpression);
        string toName = GetMemberName(toExpression);

        return ruleBuilder.Custom((instance, context) =>
        {
            DateTimeOffset from = fromSelector(instance);
            DateTimeOffset to = toSelector(instance);

            if (from >= to)
            {
                context.AddFailure(fromName, $"{fromName} must be earlier than {toName}.");
                context.AddFailure(toName, $"{toName} must be later than {fromName}.");
            }
        });
    }

    private static string GetMemberName<T, TProperty>(
        Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body is MemberExpression memberExpression)
        {
            return memberExpression.Member.Name;
        }

        throw new ArgumentException("Expresion must be a member expression.", nameof(expression));
    }
}
