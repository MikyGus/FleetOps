namespace FleetOps.Application.Vehicles.GetVehicles;

public sealed class GetVehicleByIdHandler
{
    private readonly IVehicleQueries _queries;

    public GetVehicleByIdHandler(IVehicleQueries queries) 
        => _queries = queries;

    public Task<VehicleDto?> HandleAsync(Guid id, CancellationToken ct) 
        => _queries.GetVehicleByIdAsync(id, ct);
}