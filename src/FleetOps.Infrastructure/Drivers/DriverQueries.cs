using FleetOps.Application.Drivers;
using FleetOps.Application.Drivers.GetDrivers;
using FleetOps.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FleetOps.Infrastructure.Drivers;

public sealed class DriverQueries : IDriverQueries
{
    private readonly FleetOpsDbContext _db;

    public DriverQueries(FleetOpsDbContext db)
    {
        _db = db;
    }

    public async Task<DriverDto?> GetDriverByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Drivers
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new DriverDto(
                x.Id,
                x.Name,
                x.IsActive)
            )
            .SingleOrDefaultAsync(ct);
    }

    public async Task<List<DriverDto>> GetDriversAsync(GetDriversQuery query, CancellationToken ct)
    {
        IQueryable<Domain.Drivers.Driver> drivers = _db.Drivers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            string pattern = $"%{query.Name.Trim()}%";
            drivers = drivers.Where(x => EF.Functions.ILike(x.Name, pattern));
        }

        if (query.IsActive.HasValue)
        {
            drivers = drivers.Where(x => x.IsActive == query.IsActive.Value);
        }

        return await drivers
            .OrderBy(x => x.Name)
            .Skip(query.Offset)
            .Take(query.Limit)
            .Select(x => new DriverDto(
                x.Id,
                x.Name,
                x.IsActive
            ))
            .ToListAsync(ct);
    }
}