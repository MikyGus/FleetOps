namespace FleetOps.Tests.Integration.Contracts.Entities.Vehicles;

public sealed record VehicleResponse(Guid Id, string RegistrationNumber, bool IsActive);
