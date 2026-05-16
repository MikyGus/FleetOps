using FleetOps.Domain.Assignments;
using FleetOps.Domain.Drivers;
using FleetOps.Domain.Vehicles;
using FleetOps.Infrastructure.Persistence;
using FleetOps.Tests.Integration.Contracts;
using Microsoft.Extensions.DependencyInjection;

public sealed class DbSeedBuilder
{
    private sealed record DriverSeed(string name);
    private sealed record VehicleSeed(string registrationNumber, bool isActive);
    private sealed record AssignmentSeed(string driverName, string registrationNumber, DateTimeOffset startUtc, DateTimeOffset endUtc);
    private readonly IServiceScopeFactory _scopeFactory;

    private List<DriverSeed> _drivers = [];
    private List<VehicleSeed> _vehicles = [];
    private List<AssignmentSeed> _assignments = [];

    public DbSeedBuilder(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public DbSeedBuilder SeedDriver(string name = "TestDriver")
    {
        _drivers.Add(new DriverSeed(name));
        return this;
    }

    public DbSeedBuilder SeedDrivers(int count, string namePrefix = "Driver")
    {
        for (int i = 0; i < count; i++)
        {
            _drivers.Add(new DriverSeed($"{namePrefix}{i}"));
        }
        return this;
    }

    public DbSeedBuilder SeedVehicle(string registrationNumber = "TestVehicle", bool isActive = true)
    {
        _vehicles.Add(new VehicleSeed(registrationNumber, isActive));
        return this;
    }

    public DbSeedBuilder SeedVehicles(int count, string registrationNumberPrefix = "Vehicle", bool isActive = true)
    {
        for (int i = 0; i < count; i++)
        {
            _vehicles.Add(new VehicleSeed($"{registrationNumberPrefix}{i}",isActive));
        }
        return this;
    }

    public DbSeedBuilder SeedAssignment(string driverName, string registrationNumber, DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
        if (!_drivers.Exists(x => x.name == driverName))
        {
            _drivers.Add(new DriverSeed(driverName));
        }
        if (!_vehicles.Exists(x => x.registrationNumber == registrationNumber))
        {
            _vehicles.Add(new VehicleSeed(registrationNumber, true));
        }
        _assignments.Add(new AssignmentSeed(driverName, registrationNumber, startUtc, endUtc));
        return this;
    }

    public DbSeedBuilder SeedAssignments(int count, string driverName, string registrationNumber, DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
        if (!_drivers.Exists(x => x.name == driverName))
        {
            _drivers.Add(new DriverSeed(driverName));
        }
        if (!_vehicles.Exists(x => x.registrationNumber == registrationNumber))
        {
            _vehicles.Add(new VehicleSeed(registrationNumber, true));
        }

        for (int i = 0; i < count; i++)
        {
            _assignments.Add(new AssignmentSeed(driverName, registrationNumber, startUtc, endUtc));
            startUtc = endUtc.AddHours(1);
            endUtc = startUtc.AddHours(1);
        }

        return this;
    }

    public async Task<SeedResult> SaveAsync(CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FleetOpsDbContext>();

        var result = new SeedResult();

        foreach (var seed in _drivers)
        {
            var driver = new Driver(seed.name);

            dbContext.Drivers.Add(driver);
            result.Drivers[seed.name] = driver.Id;
        }

        foreach (var seed in _vehicles)
        {
            var vehicle = new Vehicle(seed.registrationNumber);
            if (!seed.isActive)
            {
                vehicle.Deactivate();
            }

            dbContext.Vehicles.Add(vehicle);
            result.Vehicles[seed.registrationNumber] = vehicle.Id;
        }

        int assignmentIndex = 0;
        foreach (var seed in _assignments)
        {
            var assignment = new Assignment(
                result.Drivers[seed.driverName],
                result.Vehicles[seed.registrationNumber],
                seed.startUtc,
                seed.endUtc
            );

            dbContext.Assignments.Add(assignment);
            result.Assignments[$"Assignment{assignmentIndex++}"] = assignment.Id;
        }

        await dbContext.SaveChangesAsync(ct);

        return result;
    }
}

