namespace FleetOps.Application.Assignments.GetAssignments;

public interface IAssignmentQueries
{
    Task<AssignmentDto?> GetAssignmentByIdAsync(Guid id, CancellationToken ct);
    
    Task<List<AssignmentDto>> GetAssignmentsAsync(
        Guid? driverId,
        Guid? vehicleId,
        DateTimeOffset? fromUtc,
        DateTimeOffset? toUtc,
        int limit,
        int offset,
        CancellationToken ct);
}

