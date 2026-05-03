namespace FleetOps.Tests.Integration.Infrastructure.Scenarios;

public sealed record TestCreateAssignmentRequest(
    Guid DriverId,
    Guid VehicleId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc);