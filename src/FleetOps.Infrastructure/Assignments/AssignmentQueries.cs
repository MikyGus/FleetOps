using FleetOps.Application.Assignments;
using FleetOps.Application.Assignments.GetAssignments;
using FleetOps.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FleetOps.Infrastructure.Assignments;

public sealed class AssignmentQueries : IAssignmentQueries
{
    private readonly FleetOpsDbContext _db;

    public AssignmentQueries(FleetOpsDbContext db)
    {
        _db = db;
    }

    public async Task<AssignmentDto?> GetAssignmentByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Assignments
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new AssignmentDto(
                x.Id,
                x.DriverId,
                x.VehicleId,
                x.StartUtc,
                x.EndUtc))
            .SingleOrDefaultAsync(ct);
    }

    public async Task<List<AssignmentDto>> GetAssignmentsAsync(
        Guid? driverId,
        Guid? vehicleId,
        DateTimeOffset? fromUtc,
        DateTimeOffset? toUtc,
        int limit,
        int offset,
        CancellationToken ct)
    {
        IQueryable<Domain.Assignments.Assignment> query = _db.Assignments.AsNoTracking();

        if (driverId.HasValue)
        {
            query = query.Where(x => x.DriverId == driverId.Value);
        }

        if (vehicleId.HasValue)
        {
            query = query.Where(x => x.VehicleId == vehicleId.Value);
        }

        if (fromUtc.HasValue)
        {
            query = query.Where(x => x.EndUtc > fromUtc.Value);
        }

        if (toUtc.HasValue)
        {
            query = query.Where(x => x.StartUtc < toUtc.Value);
        }

        return await query
            .OrderBy(x => x.StartUtc)
            .Skip(offset)
            .Take(limit)
            .Select(x => new AssignmentDto(
                x.Id,
                x.DriverId,
                x.VehicleId,
                x.StartUtc,
                x.EndUtc))
            .ToListAsync(ct);
    }
}