
namespace FleetOps.Application.Vehicles.GetVehicles;

public interface IVehicleQueries
{
    Task<VehicleDto?> GetVehicleByIdAsync(Guid id, CancellationToken ct);
    Task<List<VehicleDto>> GetVehiclesAsync(GetVehiclesQuery query, CancellationToken ct);
}
