using FleetOps.Application.Vehicles;
using FleetOps.Domain.Vehicles;
using FleetOps.Infrastructure.Persistence;

namespace FleetOps.Infrastructure.Vehicles;

public sealed class VehicleRepository : IVehicleRepository
{
    private readonly FleetOpsDbContext _db;

    public VehicleRepository(FleetOpsDbContext db)
    {
        _db = db;
    }

    public Task AddAsync(Vehicle vehicle, CancellationToken ct)
        => _db.Vehicles.AddAsync(vehicle, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}