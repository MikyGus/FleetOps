using FleetOps.Application.Validations;
using FleetOps.Domain.Drivers;
using FleetOps.Domain.Vehicles;
using FluentValidation;

namespace FleetOps.Application.Assignments.CreateAssignment;

public sealed class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator(
        IEntityExistenceChecker<Driver, Guid> driverExistenceChecker,
        IEntityExistenceChecker<Vehicle, Guid> vehicleExistenceChecker)
    {
        RuleFor(x => x.DriverId).ValidateEntityExists(driverExistenceChecker, "Driver");
        RuleFor(x => x.VehicleId).ValidateEntityExists(vehicleExistenceChecker, "Vehicle");

        RuleFor(x => x).ValidDateOrder(x => x.StartUtc, x => x.EndUtc);
    }
}