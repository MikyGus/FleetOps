namespace FleetOps.Application.Assignments.GetAssignments;

public sealed record AssignmentDto(
    Guid Id,
    Guid DriverId,
    Guid VehicleId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc
);