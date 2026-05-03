using FleetOps.Domain.Drivers;
using FleetOps.Domain.Vehicles;
using FleetOps.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace FleetOps.Tests.Integration.Infrastructure.Database;

public sealed class TestDatabaseSeeder
{
    private readonly IServiceScopeFactory _scopeFactory;

    public TestDatabaseSeeder(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<Guid> SeedDriver(string name = "TestDriver", CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FleetOpsDbContext>();

        var driver = new Driver(name);

        dbContext.Drivers.Add(driver);

        await dbContext.SaveChangesAsync(ct);

        return driver.Id;
    }

    public async Task<Guid> SeedVehicle(string registrationNumber = "TestVehicle", bool isActive = true, CancellationToken ct = default)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<FleetOpsDbContext>();

        var vehicle = new Vehicle(registrationNumber);
        if (!isActive)
        {
            vehicle.Deactivate();
        }

        dbContext.Vehicles.Add(vehicle);

        await dbContext.SaveChangesAsync(ct);

        return vehicle.Id;
    }
}