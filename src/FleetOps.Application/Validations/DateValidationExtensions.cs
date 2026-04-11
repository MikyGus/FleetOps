using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;

namespace FleetOps.Application.Validations;

public static class DateValidationExtensions
{
    public static IRuleBuilderOptionsConditions<T, T> ValidDateOrder<T>(
        this IRuleBuilderInitial<T, T> ruleBuilder,
        Expression<Func<T, DateTimeOffset?>> fromExpression,
        Expression<Func<T, DateTimeOffset?>> toExpression,
        string errorCode)
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
                context.AddFailure(new ValidationFailure(
                    fromName, 
                    $"{fromName} must be earlier than {toName}.")
                {
                    ErrorCode = errorCode
                });

                context.AddFailure(new ValidationFailure(
                    toName,
                    $"{toName} must be later than {fromName}.")
                {
                    ErrorCode = errorCode
                });
            }
        });
    }

    public static IRuleBuilderOptionsConditions<T, T> ValidDateOrder<T>(
        this IRuleBuilderInitial<T, T> ruleBuilder,
        Expression<Func<T, DateTimeOffset>> fromExpression,
        Expression<Func<T, DateTimeOffset>> toExpression,
        string errorCode)
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
                context.AddFailure(new ValidationFailure(
                    fromName, 
                    $"{fromName} must be earlier than {toName}.")
                {
                    ErrorCode = errorCode
                });

                context.AddFailure(new ValidationFailure(
                    toName,
                    $"{toName} must be later than {fromName}.")
                {
                    ErrorCode = errorCode
                });
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