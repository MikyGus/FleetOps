using FleetOps.Domain.Vehicles;

namespace FleetOps.Application.Vehicles;

public interface IVehicleRepository
{
    Task AddAsync(Vehicle vehicle, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
