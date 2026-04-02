using FleetOps.Application.Validations;
using FleetOps.Domain.Drivers;
using FluentValidation;

namespace FleetOps.Application.Assignments.CreateAssignment;

public sealed class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator(
        IEntityExistenceChecker<Driver, Guid> driverExistenceChecker)
    {
        RuleFor(x => x.DriverId).ValidateEntityExists(driverExistenceChecker, "Driver");
        RuleFor(x => x.VehicleId).ValidRequiredId();

        RuleFor(x => x).ValidDateOrder(x => x.StartUtc, x => x.EndUtc);
    }
}