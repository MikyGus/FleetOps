using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.TestHelper;

namespace FleetOps.Tests.Unit.Application.Common.Validation;

public static class ValidationAssert
{
    public static async Task HasError<T, TProperty>(
        IValidator<T> validator,
        T instance,
        Expression<Func<T, TProperty>> expression,
        string errorCode
    )
    {
        var result = await validator.TestValidateAsync(instance);

        result.ShouldHaveValidationErrorFor(expression)
            .WithErrorCode(errorCode);
    }

    public static async Task HasNoError<T, TProperty>(
        IValidator<T> validator,
        T instance,
        Expression<Func<T, TProperty>> expression
    )
    {
        var result = await validator.TestValidateAsync(instance);

        result.ShouldNotHaveValidationErrorFor(expression);
    }
}