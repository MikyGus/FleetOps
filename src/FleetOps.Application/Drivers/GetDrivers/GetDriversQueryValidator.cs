using System.IO.Compression;
using FluentValidation;

namespace FleetOps.Application.Drivers.GetDrivers;

public sealed class GetDriversQueryValidator : AbstractValidator<GetDriversQuery>
{
    public GetDriversQueryValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0)
            .LessThanOrEqualTo(500);

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Name));
    }
}