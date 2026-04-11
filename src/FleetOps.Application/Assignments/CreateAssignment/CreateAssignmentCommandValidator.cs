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
        RuleFor(x => x.DriverId).ValidateEntityExists(
            driverExistenceChecker,
            "Driver",
            ValidationErrorCodes.Assignment.DriverId.Required,
            ValidationErrorCodes.Assignment.DriverId.NotFound);

        RuleFor(x => x.VehicleId).ValidateEntityExists(
            vehicleExistenceChecker,
            "Vehicle",
            ValidationErrorCodes.Assignment.VehicleId.Required,
            ValidationErrorCodes.Assignment.VehicleId.NotFound);

        RuleFor(x => x)
            .ValidDateOrder(x => x.StartUtc, x => x.EndUtc,
                ValidationErrorCodes.Assignment.TimeRange.Invalid);
    }
}