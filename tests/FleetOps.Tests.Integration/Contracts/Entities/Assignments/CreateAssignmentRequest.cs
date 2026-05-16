namespace FleetOps.Tests.Integration.Contracts.Assignments;

public sealed record CreateAssignmentRequest(
    Guid DriverId,
    Guid VehicleId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc);