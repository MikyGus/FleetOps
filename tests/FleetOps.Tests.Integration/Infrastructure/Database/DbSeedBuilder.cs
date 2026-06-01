using FleetOps.Domain.Assignments;
using FleetOps.Domain.Drivers;
using FleetOps.Domain.Vehicles;
using FleetOps.Infrastructure.Persistence;
using FleetOps.Tests.Integration.Contracts;
using FleetOps.Tests.Integration.Infrastructure.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace FleetOps.Tests.Integration.Infrastructure.Database;

public sealed class DbSeedBuilder
{
    private sealed record DriverSeed(string Name);
    private sealed record VehicleSeed(string RegistrationNumber, bool IsActive);
    private sealed record AssignmentSeed(string DriverName, string RegistrationNumber, DateTimeOffset StartUtc, DateTimeOffset EndUtc);
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly List<DriverSeed> _drivers = [];
    private readonly List<VehicleSeed> _vehicles = [];
    private readonly List<AssignmentSeed> _assignments = [];

    [Flags]
    public enum AffixPosition
    {
        Prefix = 1,
        Postfix = 2
    }

    public DbSeedBuilder(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public DbSeedBuilder SeedDriver(string name = "TestDriver")
    {
        _drivers.Add(new DriverSeed(name));
        return this;
    }

    public DbSeedBuilder SeedDrivers(int count, string name = "Driver", AffixPosition affixPosition = AffixPosition.Postfix)
    {
        for (int i = 0; i < count; i++)
        {
            var prefix = (affixPosition & AffixPosition.Prefix) != 0 ? i.ToString() : "";
            var postfix = (affixPosition & AffixPosition.Postfix) != 0 ? i.ToString() : "";

            _drivers.Add(new DriverSeed($"{prefix}{name}{postfix}"));
        }
        return this;
    }

    public DbSeedBuilder SeedVehicle(string registrationNumber = "TestVehicle", bool isActive = true)
    {
        _vehicles.Add(new VehicleSeed(registrationNumber, isActive));
        return this;
    }

    public DbSeedBuilder SeedVehicles(int count, string registrationNumberPrefix = "Vehicle", bool isActive = true, AffixPosition affixPosition = AffixPosition.Postfix)
    {
        for (int i = 0; i < count; i++)
        {
            var prefix = (affixPosition & AffixPosition.Prefix) != 0 ? i.ToString() : "";
            var postfix = (affixPosition & AffixPosition.Postfix) != 0 ? i.ToString() : "";

            _vehicles.Add(new VehicleSeed($"{prefix}{registrationNumberPrefix}{postfix}", isActive));
        }
        return this;
    }

    public DbSeedBuilder SeedAssignment(string driverName, string registrationNumber)
        => SeedAssignment(driverName, registrationNumber, TimeTestFixtures.Period1.Start, TimeTestFixtures.Period1.End_Valid);

    public DbSeedBuilder SeedAssignment(string driverName, string registrationNumber, DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
        if (!_drivers.Exists(x => x.Name == driverName))
        {
            _drivers.Add(new DriverSeed(driverName));
        }
        if (!_vehicles.Exists(x => x.RegistrationNumber == registrationNumber))
        {
            _vehicles.Add(new VehicleSeed(registrationNumber, true));
        }
        _assignments.Add(new AssignmentSeed(driverName, registrationNumber, startUtc, endUtc));
        return this;
    }

    public DbSeedBuilder SeedAssignments(int count, string driverName, string registrationNumber)
        => SeedAssignments(count, driverName, registrationNumber, TimeTestFixtures.Period1.Start, TimeTestFixtures.Period1.End_Valid);

    public DbSeedBuilder SeedAssignments(int count, string driverName, string registrationNumber, DateTimeOffset startUtc, DateTimeOffset endUtc)
    {
        if (!_drivers.Exists(x => x.Name == driverName))
        {
            _drivers.Add(new DriverSeed(driverName));
        }
        if (!_vehicles.Exists(x => x.RegistrationNumber == registrationNumber))
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
            var driver = new Driver(seed.Name);

            dbContext.Drivers.Add(driver);
            result.Drivers[seed.Name] = driver;
        }

        foreach (var seed in _vehicles)
        {
            var vehicle = new Vehicle(seed.RegistrationNumber);
            if (!seed.IsActive)
            {
                vehicle.Deactivate();
            }

            dbContext.Vehicles.Add(vehicle);
            result.Vehicles[seed.RegistrationNumber] = vehicle;
        }

        int assignmentIndex = 0;
        foreach (var seed in _assignments)
        {
            var assignment = new Assignment(
                result.Drivers[seed.DriverName].Id,
                result.Vehicles[seed.RegistrationNumber].Id,
                seed.StartUtc,
                seed.EndUtc
            );

            dbContext.Assignments.Add(assignment);
            result.Assignments[$"Assignment{assignmentIndex++}"] = assignment;
        }

        await dbContext.SaveChangesAsync(ct);

        return result;
    }
}

