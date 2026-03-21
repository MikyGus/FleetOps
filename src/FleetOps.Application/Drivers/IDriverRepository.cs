using FleetOps.Domain.Drivers;

namespace FleetOps.Application.Drivers;

public interface IDriverRepository
{
    Task AddAsync(Driver driver, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}