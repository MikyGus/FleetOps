using FleetOps.Application.Validations;
using FluentValidation;

namespace FleetOps.Application.Vehicles.GetVehicles;

public sealed class GetVehiclesQueryValidator : AbstractValidator<GetVehiclesQuery>
{
    public GetVehiclesQueryValidator()
    {
        RuleFor(x => x.Limit).ValidLimit();
        RuleFor(x => x.Offset).ValidOffset();
        RuleFor(x => x.RegistrationNumber).ValidRegistrationNumberOptional();
    }
}