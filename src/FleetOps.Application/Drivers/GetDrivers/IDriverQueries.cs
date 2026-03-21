namespace FleetOps.Application.Drivers.GetDrivers;

public interface IDriverQueries
{
    Task<List<DriverDto>> GetDriversAsync(GetDriversQuery query, CancellationToken ct);
    Task<DriverDto?> GetDriverByIdAsync(Guid id, CancellationToken ct);
}