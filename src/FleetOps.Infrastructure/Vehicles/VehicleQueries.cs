using FleetOps.Application.Vehicles;
using FleetOps.Application.Vehicles.GetVehicles;
using FleetOps.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FleetOps.Infrastructure.Vehicles;

public sealed class VehicleQueries : IVehicleQueries
{
    private readonly FleetOpsDbContext _db;

    public VehicleQueries(FleetOpsDbContext db)
    {
        _db = db;
    }

    public async Task<VehicleDto?> GetVehicleByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Vehicles
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new VehicleDto(
                x.Id,
                x.RegistrationNumber,
                x.IsActive)
            )
            .SingleOrDefaultAsync(ct);
    }
}