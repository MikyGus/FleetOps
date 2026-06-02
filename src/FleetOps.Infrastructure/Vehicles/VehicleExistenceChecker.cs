using FleetOps.Application.Validations;
using FleetOps.Domain.Vehicles;
using FleetOps.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FleetOps.Infrastructure.Vehicles;

public sealed class VehicleExistenceChecker : IEntityExistenceChecker<Vehicle, Guid>
{
    private readonly FleetOpsDbContext _db;

    public VehicleExistenceChecker(FleetOpsDbContext db) => _db = db;

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        => _db.Vehicles.AsNoTracking().AnyAsync(x => x.Id == id, ct);
}
