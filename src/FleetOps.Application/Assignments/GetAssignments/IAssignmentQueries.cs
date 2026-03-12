namespace FleetOps.Application.Assignments.GetAssignments;

public interface IAssignmentQueries
{
    Task<List<AssignmentDto>> GetAssignmentsAsync(
        Guid? driverId,
        Guid? vehicleId,
        DateTimeOffset? fromUtc,
        DateTimeOffset? toUtc,
        int limit,
        int offset,
        CancellationToken ct);
}

