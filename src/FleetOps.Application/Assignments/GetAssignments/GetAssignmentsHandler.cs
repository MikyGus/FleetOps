namespace FleetOps.Application.Assignments.GetAssignments;

public sealed class GetAssignmentsHandler
{
    private readonly IAssignmentQueries _queries;

    public GetAssignmentsHandler(IAssignmentQueries queries)
    {
        _queries = queries;
    }

    public Task<List<AssignmentDto>> HandleAsync(GetAssignmentsQuery query, CancellationToken ct)
    {
        return _queries.GetAssignmentsAsync(
            query.DriverId,
            query.VehicleId,
            query.FromUtc,
            query.ToUtc,
            query.Limit,
            query.Offset,
            ct);
    }
}