using FluentValidation;

namespace FleetOps.Application.Validations;

public static class IdentifierValidationExtensions
{
    public static IRuleBuilderOptions<T, Guid> ValidateEntityExists<T, TEntity>(
        this IRuleBuilderInitial<T, Guid> ruleBuilder,
        IEntityExistenceChecker<TEntity, Guid> existenceChecker,
        string entityName,
        string requiredErrorCode,
        string notfoundErrorCode)
    {
        return ruleBuilder
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("{PropertyName} must be specified.")
                .WithName(entityName)
                .WithErrorCode(requiredErrorCode)
            .MustAsync(async (id, ct) =>
                    await existenceChecker.ExistsAsync(id, ct))
                .WithName(entityName)
                .WithMessage("{PropertyName} does not exist.")
                .WithErrorCode(notfoundErrorCode);
    }
}