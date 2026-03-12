namespace FleetOps.Application.Assignments.GetAssignments;

public sealed record GetAssignmentsQuery(
    Guid? DriverId,
    Guid? VehicleId,
    DateTimeOffset? FromUtc,
    DateTimeOffset? ToUtc,
    int Limit = 50,
    int Offset = 0
);