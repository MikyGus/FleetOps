using System.IO.Compression;
using FluentValidation;

namespace FleetOps.Application.Drivers.GetDrivers;

public sealed class GetDriversQueryValidator : AbstractValidator<GetDriversQuery>
{
    public GetDriversQueryValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1,500)
            .WithMessage("{PropertyName} must be between {From} and {To}.");

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0)
            .WithMessage("{PropertyName} must be greater than or equal to {ComparisonValue}.");

        RuleFor(x => x.Name)
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Name))
            .WithMessage("{PropertyName} is {TotalLength}, but must be at most {MaxLength} characters long.");
    }
}