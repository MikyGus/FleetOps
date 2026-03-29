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

    public async Task<List<VehicleDto>> GetVehiclesAsync(GetVehiclesQuery query, CancellationToken ct)
    {
        IQueryable<Domain.Vehicles.Vehicle> vehicles = _db.Vehicles.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.RegistrationNumber))
        {
            string pattern = $"%{query.RegistrationNumber.Trim()}%";
            vehicles = vehicles.Where(x => EF.Functions.ILike(x.RegistrationNumber, pattern));
        }

        if (query.IsActive.HasValue)
        {
            vehicles = vehicles.Where(x => x.IsActive == query.IsActive.Value);
        }

        return await vehicles
            .OrderBy(x => x.RegistrationNumber)
            .Skip(query.Offset)
            .Take(query.Limit)
            .Select(x => new VehicleDto(
                x.Id,
                x.RegistrationNumber,
                x.IsActive)
            )
            .ToListAsync();
    }
}