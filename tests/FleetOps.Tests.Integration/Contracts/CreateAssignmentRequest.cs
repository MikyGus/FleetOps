namespace FleetOps.Tests.Integration.Contracts;

public sealed record CreateAssignmentRequest(
    Guid DriverId,
    Guid VehicleId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc);