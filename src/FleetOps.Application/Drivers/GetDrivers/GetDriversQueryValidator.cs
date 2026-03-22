using FleetOps.Application.Validations;
using FluentValidation;

namespace FleetOps.Application.Drivers.GetDrivers;

public sealed class GetDriversQueryValidator : AbstractValidator<GetDriversQuery>
{
    public GetDriversQueryValidator()
    {
        RuleFor(x => x.Limit).ValidLimit();

        RuleFor(x => x.Offset).ValidOffset();

        RuleFor(x => x.Name)
            .MaxNameLength()
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}