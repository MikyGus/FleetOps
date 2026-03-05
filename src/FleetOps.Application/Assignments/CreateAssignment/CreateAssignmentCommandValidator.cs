using FluentValidation;

namespace FleetOps.Application.Assignments.CreateAssignment;

public sealed class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
        RuleFor(x => x.VehicleId).NotEmpty();

        RuleFor(x => x.EndUtc)
            .GreaterThan(x => x.StartUtc)
            .WithMessage("EndUtc must be greater than StartUtc.");
    }
}