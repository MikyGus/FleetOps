using FleetOps.Application.Validations;
using FluentValidation;

namespace FleetOps.Application.Assignments.CreateAssignment;

public sealed class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator()
    {
        RuleFor(x => x.DriverId).ValidRequiredId();
        RuleFor(x => x.VehicleId).ValidRequiredId();

        RuleFor(x => x).ValidDateOrder(x => x.StartUtc, x => x.EndUtc);
    }
}