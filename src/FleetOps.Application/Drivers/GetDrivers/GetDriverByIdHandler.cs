namespace FleetOps.Application.Drivers.GetDrivers;

public sealed class GetDriverByIdHandler
{
    private readonly IDriverQueries _queries;

    public GetDriverByIdHandler(IDriverQueries queries)
    {
        _queries = queries;
    }

    public Task<DriverDto?> HandleAsync(Guid id, CancellationToken ct)
    {
        return _queries.GetDriverByIdAsync(id, ct);
    } 
}