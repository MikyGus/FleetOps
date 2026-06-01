using FleetOps.Application.Validations;
using FleetOps.Domain.Drivers;
using FleetOps.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FleetOps.Infrastructure.Drivers;

public sealed class DriverExistenceChecker : IEntityExistenceChecker<Driver, Guid>
{
    private readonly FleetOpsDbContext _db;

    public DriverExistenceChecker(FleetOpsDbContext db)
        => _db = db;

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct)
        => _db.Drivers.AsNoTracking().AnyAsync(x => x.Id == id, ct);
}