namespace FleetOps.Application.Vehicles.GetVehicles;

public interface IVehicleQueries
{
    Task<VehicleDto?> GetVehicleByIdAsync(Guid id, CancellationToken ct);
}