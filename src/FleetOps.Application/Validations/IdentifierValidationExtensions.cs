using FluentValidation;

namespace FleetOps.Application.Validations;

public static class IdentifierValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid> ValidateEntityExists<T, TEntity>(
        this IRuleBuilderInitial<T, Guid> ruleBuilder,
        IEntityExistenceChecker<TEntity, Guid> existenceChecker,
        string entityName)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("{PropertyName} must be specified.")
                .WithName(entityName)
            .MustAsync(async (id, ct) =>
                    await existenceChecker.ExistsAsync(id, ct))
                .WithName(entityName)
                .WithMessage("{PropertyName} does not exist.");
    }


    public static IRuleBuilderOptions<T, Guid> ValidRequiredId<T>(this IRuleBuilder<T, Guid> ruleBuilder)
        => ruleBuilder
            .NotEmpty()
            .WithMessage("{PropertyName} must not be a non-empty GUID.");
}