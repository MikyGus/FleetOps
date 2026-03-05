namespace FleetOps.Application.Assignments.CreateAssignment;

public sealed record CreateAssignmentCommand(
    Guid DriverId,
    Guid VehicleId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc
);