public sealed class Assignment
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid DriverId { get; private set; }
    public Guid VehicleId { get; private set; }

    public DateTimeOffset StartUtc { get; private set; }
    public DateTimeOffset EndUtc { get; private set; }

    private Assignment() {} // For ORM

    public Assignment(Guid driverId, Guid vehicleId, DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
        if (driverId == Guid.Empty) throw new ArgumentException("DriverId must be set.", nameof(driverId));
        if (vehicleId == Guid.Empty) throw new ArgumentException("VehicleId must be set.", nameof(vehicleId));
        if (endUtc <= startUtc) throw new ArgumentException("EndUtc must be greater than StartUtc.", nameof(endUtc));

        DriverId = driverId;
        VehicleId = vehicleId;
        StartUtc = startUtc;
        EndUtc = endUtc;
    }
}
