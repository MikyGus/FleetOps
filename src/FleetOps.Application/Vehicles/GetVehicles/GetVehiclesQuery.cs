namespace FleetOps.Application.Vehicles.GetVehicles;

public sealed record GetVehiclesQuery(
    string? RegistrationNumber,
    bool? IsActive,
    int Limit = 50,
    int Offset = 0
);
