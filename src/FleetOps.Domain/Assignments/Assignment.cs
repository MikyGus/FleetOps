using FleetOps.Domain.Drivers;
using FleetOps.Domain.Errors;
using FleetOps.Domain.Exceptions;
using FleetOps.Domain.Vehicles;

namespace FleetOps.Domain.Assignments;

public sealed class Assignment
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid DriverId { get; private set; }
    public Guid VehicleId { get; private set; }


    public DateTimeOffset StartUtc { get; private set; }
    public DateTimeOffset EndUtc { get; private set; }

    public Driver Driver { get; private set; } = null!;
    public Vehicle Vehicle { get; private set; } = null!;

    private Assignment() { } // For ORM

    public Assignment(Guid driverId, Guid vehicleId, DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
        if (driverId == Guid.Empty)
            throw new DomainValidationException(nameof(driverId), ErrorCodes.Assignment.DriverId.Required, "DriverId must be set.");
        if (vehicleId == Guid.Empty)
            throw new DomainValidationException(nameof(vehicleId), ErrorCodes.Assignment.VehicleId.Required, "VehicleId must be set.");
        if (endUtc <= startUtc)
            throw new DomainValidationException(nameof(endUtc), ErrorCodes.Assignment.TimeRange.Invalid, "EndUtc must be greater than StartUtc.");

        DriverId = driverId;
        VehicleId = vehicleId;
        StartUtc = startUtc;
        EndUtc = endUtc;
    }
}
