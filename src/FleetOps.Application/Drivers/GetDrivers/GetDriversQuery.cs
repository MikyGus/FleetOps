namespace FleetOps.Application.Drivers.GetDrivers;

public sealed record GetDriversQuery(
    string? Name,
    bool? IsActive,
    int Limit = 50,
    int Offset = 0
);