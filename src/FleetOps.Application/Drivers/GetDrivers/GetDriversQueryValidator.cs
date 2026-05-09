using FleetOps.Application.Validations;
using FleetOps.Domain.Errors;
using FluentValidation;

namespace FleetOps.Application.Drivers.GetDrivers;

public sealed class GetDriversQueryValidator : AbstractValidator<GetDriversQuery>
{
    public GetDriversQueryValidator()
    {
        RuleFor(x => x.Limit).ValidLimit();

        RuleFor(x => x.Offset).ValidOffset();

        RuleFor(x => x.Name)
            .MaxNameLength(ErrorCodes.Driver.Name.MaxLengthExceeded)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}