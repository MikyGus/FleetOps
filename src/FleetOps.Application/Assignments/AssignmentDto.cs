namespace FleetOps.Application.Assignments;

public sealed record AssignmentDto(
    Guid Id,
    Guid DriverId,
    Guid VehicleId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc
);