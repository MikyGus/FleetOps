using FleetOps.Application.Validations;
using FluentValidation;

namespace FleetOps.Application.Assignments.GetAssignments;

public sealed class GetAssignmentsQueryValidator : AbstractValidator<GetAssignmentsQuery>
{
    public GetAssignmentsQueryValidator()
    {
        RuleFor(x => x.Limit).ValidLimit();

        RuleFor(x => x.Offset).ValidOffset();

        RuleFor(x => x).ValidDateOrder(x => x.FromUtc, x => x.ToUtc);
    }
}