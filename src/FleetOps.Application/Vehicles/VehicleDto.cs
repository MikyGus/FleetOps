namespace FleetOps.Application.Vehicles;

public sealed record VehicleDto(Guid Id, string RegistrationNumber, bool IsActive);