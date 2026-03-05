namespace FleetOps.Api.Contracts.Assignments;

public sealed record CreateAssignmentRequest(
    Guid DriverId,
    Guid VehicleId,
    DateTimeOffset StartUtc,
    DateTimeOffset EndUtc);