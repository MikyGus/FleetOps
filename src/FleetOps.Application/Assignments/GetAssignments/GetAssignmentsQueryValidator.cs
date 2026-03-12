using FluentValidation;

namespace FleetOps.Application.Assignments.GetAssignments;

public sealed class GetAssignmentsQueryValidator : AbstractValidator<GetAssignmentsQuery>
{
    public GetAssignmentsQueryValidator()
    {
        RuleFor(x => x.Limit)
            .InclusiveBetween(1,500)
            .WithMessage("{PropertyName} must be between {From} and {To}.");

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0)
            .WithMessage("{PropertyName} must be greater than or equal to {ComparisonValue}.");

        RuleFor(x => x).Custom((x, context) =>
        {
            if (x.FromUtc.HasValue &&
                x.ToUtc.HasValue &&
                x.FromUtc >= x.ToUtc)
            {
                context.AddFailure("fromUtc", "fromUtc must be earlier than toUtc.");
                context.AddFailure("toUtc", "toUtc must be later than fromUtc.");
            }
        });
    }
}