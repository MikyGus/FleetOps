using FluentValidation;

namespace FleetOps.Application.Vehicles.GetVehicles;

public sealed class GetVehiclesHandler
{
    private readonly IVehicleQueries _queries;
    private readonly IValidator<GetVehiclesQuery> _validator;

    public GetVehiclesHandler(IVehicleQueries queries, IValidator<GetVehiclesQuery> validator)
    {
        _queries = queries;
        _validator = validator;
    }

    public async Task<List<VehicleDto>> HandleAsync(GetVehiclesQuery query, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(query, ct);
        return await _queries.GetVehiclesAsync(query, ct);
    }
}
