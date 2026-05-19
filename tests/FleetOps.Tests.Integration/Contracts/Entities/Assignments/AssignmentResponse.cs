namespace FleetOps.Tests.Integration.Contracts.Assignments;

public sealed record AssignmentResponse(
    Guid Id,
    Guid DriverId,
    Guid VehicleId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc
);