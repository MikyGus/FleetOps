using FleetOps.Application.Validations;
using FleetOps.Domain.Drivers;
using FleetOps.Domain.Errors;
using FleetOps.Domain.Vehicles;
using FluentValidation;

namespace FleetOps.Application.Assignments.CreateAssignment;

public sealed class CreateAssignmentCommandValidator : AbstractValidator<CreateAssignmentCommand>
{
    public CreateAssignmentCommandValidator(
        IEntityExistenceChecker<Driver, Guid> driverExistenceChecker,
        IEntityExistenceChecker<Vehicle, Guid> vehicleExistenceChecker)
    {
        RuleFor(x => x.DriverId).ValidateEntityExists(
            driverExistenceChecker,
            "Driver",
            ErrorCodes.Assignment.DriverId.Required,
            ErrorCodes.Assignment.DriverId.NotFound);

        RuleFor(x => x.VehicleId).ValidateEntityExists(
            vehicleExistenceChecker,
            "Vehicle",
            ErrorCodes.Assignment.VehicleId.Required,
            ErrorCodes.Assignment.VehicleId.NotFound);

        RuleFor(x => x)
            .ValidDateOrder(x => x.StartUtc, x => x.EndUtc,
                ErrorCodes.Assignment.TimeRange.Invalid);
    }
}