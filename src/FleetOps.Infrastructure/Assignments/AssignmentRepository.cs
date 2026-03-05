using FleetOps.Application.Assignments;
using FleetOps.Domain.Assignments;
using FleetOps.Infrastructure.Persistence;

namespace FleetOps.Infrastructure.Assignments;

public sealed class AssignmentRepository : IAssignmentRepository
{
    private readonly FleetOpsDbContext _db;

    public AssignmentRepository(FleetOpsDbContext db)
    {
        _db = db;
    }

    public Task AddAsync(Assignment assignment, CancellationToken ct)
        => _db.Assignments.AddAsync(assignment, ct).AsTask();

    public Task SaveChangesAsync(CancellationToken ct)
        => _db.SaveChangesAsync(ct);
}