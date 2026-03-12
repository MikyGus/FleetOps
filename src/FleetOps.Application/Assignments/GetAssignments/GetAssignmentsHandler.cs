using FluentValidation;

namespace FleetOps.Application.Assignments.GetAssignments;

public sealed class GetAssignmentsHandler
{
    private readonly IAssignmentQueries _queries;
    private readonly IValidator<GetAssignmentsQuery> _validator;

    public GetAssignmentsHandler(
        IAssignmentQueries queries,
        IValidator<GetAssignmentsQuery> validator)
    {
        _queries = queries;
        _validator = validator;
    }

    public async Task<List<AssignmentDto>> HandleAsync(GetAssignmentsQuery query, CancellationToken ct)
    {
        var validation =  await _validator.ValidateAsync(query, ct);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        return await _queries.GetAssignmentsAsync(
            query.DriverId,
            query.VehicleId,
            query.FromUtc,
            query.ToUtc,
            query.Limit,
            query.Offset,
            ct);
    }
}