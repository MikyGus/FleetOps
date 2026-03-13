namespace FleetOps.Application.Assignments.GetAssignments;

public sealed class GetAssignmentByIdHandler
{
    private readonly IAssignmentQueries _queries;

    public GetAssignmentByIdHandler(IAssignmentQueries queries)
    {
        _queries = queries;
    }

    public Task<AssignmentDto?> HandleAsync(Guid id, CancellationToken ct)
    {
        return _queries.GetAssignmentByIdAsync(id, ct);
    }
}