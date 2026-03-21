using FleetOps.Application.Drivers;
using FleetOps.Domain.Drivers;
using FleetOps.Infrastructure.Persistence;

namespace FleetOps.Infrastructure.Drivers;

public sealed class DriverRepository : IDriverRepository
{
    private readonly FleetOpsDbContext _db;

    public DriverRepository(FleetOpsDbContext db)
    {
        _db = db;
    }

    public Task AddAsync(Driver driver, CancellationToken ct)
        => _db.Drivers.AddAsync(driver, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}