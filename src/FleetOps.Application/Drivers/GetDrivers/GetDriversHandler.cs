using FluentValidation;

namespace FleetOps.Application.Drivers.GetDrivers;

public sealed class GetDriversHandler
{
    private readonly IDriverQueries _queries;
    private readonly IValidator<GetDriversQuery> _validator;

    public GetDriversHandler(IDriverQueries queries, IValidator<GetDriversQuery> validator)
    {
        _queries = queries;
        _validator = validator;
    }

    public async Task<List<DriverDto>> HandleAsync(GetDriversQuery query, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(query, ct);
        return await _queries.GetDriversAsync(query, ct);
    }
}