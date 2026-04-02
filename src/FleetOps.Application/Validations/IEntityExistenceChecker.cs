namespace FleetOps.Application.Validations;

public interface IEntityExistenceChecker<TEntity, TId>
{
    Task<bool> ExistsAsync(TId id, CancellationToken ct);
}