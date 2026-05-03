using System.Net;
using System.Net.Http.Json;
using FleetOps.Tests.Integration.Infrastructure.Database;
using FleetOps.Tests.Integration.Infrastructure.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace FleetOps.Tests.Integration.Application.Assignments;

public sealed class CreateAssignmentTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private TestDatabaseSeeder _dbSeeder;

    public CreateAssignmentTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
       _dbSeeder = new TestDatabaseSeeder(factory.Services.GetRequiredService<IServiceScopeFactory>());
    }

    public async Task InitializeAsync() => await TestDatabaseCleaner.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Should_return_400_when_driver_does_not_exist()
    {
        var vehicleId = await _dbSeeder.SeedVehicle("Vehicle1", true);

        var request = new
        {
          driverId = Guid.NewGuid(),
          vehicleId,
          startUtc =  TimeTestFixtures.Period1.Start,
          endUtc = TimeTestFixtures.Period1.End_Valid
        };

        var response = await _client.PostAsJsonAsync("/assignments", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_return_400_when_vehicle_does_not_exist()
    {
        var driverId = await _dbSeeder.SeedDriver("Driver1");

        var request = new
        {
            driverId,
            vehicleId = Guid.NewGuid(),
          startUtc =  TimeTestFixtures.Period1.Start,
          endUtc = TimeTestFixtures.Period1.End_Valid
        };

        var response = await _client.PostAsJsonAsync("/assignments", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_return_400_when_endtime_is_before_starttime()
    {
        var driverId = await _dbSeeder.SeedDriver("Driver");
        var vehicleId = await _dbSeeder.SeedVehicle("Vehicle1");

        var request = new
        {
            driverId,
            vehicleId,
            startUtc = TimeTestFixtures.Period1.Start,
            endUtc = TimeTestFixtures.Period1.End_Invalid_BeforeStart
        };

        var response = await _client.PostAsJsonAsync("/assignments", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_return_409_when_driver_has_overlapping_assignments()
    {
        // Arrange
        var driver1Id = await _dbSeeder.SeedDriver("Driver1");
        var vehicle1Id = await _dbSeeder.SeedVehicle("Vehicle1");
        var vehicle2Id = await _dbSeeder.SeedVehicle("Vehicle2");

        var assignment1 = new
        {
            driverId = driver1Id,
            vehicleId = vehicle1Id,
            startUtc =  TimeTestFixtures.Period1.Start,
            endUtc = TimeTestFixtures.Period1.End_Valid
        };
        _ = await _client.PostAsJsonAsync("/assignments", assignment1);

        // Act
        var request = new
        {
            driverId = driver1Id,
            vehicleId = vehicle2Id,
            startUtc = TimeTestFixtures.Period2.Start_Invalid_ConflictWithPeriod1,
            endUtc = TimeTestFixtures.Period2.End_Valid
        };

        var response = await _client.PostAsJsonAsync("/assignments", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Should_return_409_when_vehicle_has_overlapping_assignments()
    {
        // Arrange
        var driver1Id = await _dbSeeder.SeedDriver("Driver1");
        var driver2Id = await _dbSeeder.SeedDriver("Driver2");
        var vehicle1Id = await _dbSeeder.SeedVehicle("Vehicle1");

        var assignment1 = new
        {
            driverId = driver1Id,
            vehicleId = vehicle1Id,
            startUtc = TimeTestFixtures.Period1.Start,
            endUtc = TimeTestFixtures.Period1.End_Valid
        };

        _ = await _client.PostAsJsonAsync("/assignments", assignment1);

        // Act
        var request = new
        {
            driverId = driver2Id,
            vehicleId = vehicle1Id,
            startUtc = TimeTestFixtures.Period2.Start_Invalid_ConflictWithPeriod1,
            endUtc = TimeTestFixtures.Period2.End_Valid
        };

        var response = await _client.PostAsJsonAsync("/assignments", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Should_return_201_when_assignments_time_overlaps_but_driver_and_vehicle_are_different()
    {
        // Arrange
        var driver1Id = await _dbSeeder.SeedDriver("Driver1");
        var driver2Id = await _dbSeeder.SeedDriver("Driver2");
        var vehicle1Id = await _dbSeeder.SeedVehicle("Vehicle1");
        var vehicle2Id = await _dbSeeder.SeedVehicle("Vehicle2");

        var assignment1 = new
        {
            driverId = driver1Id,
            vehicleId = vehicle1Id,
            startUtc = TimeTestFixtures.Period1.Start,
            endUtc = TimeTestFixtures.Period1.End_Valid
        };

        _ = await _client.PostAsJsonAsync("/assignments", assignment1);

        // Act
        var request = new
        {
            driverId = driver2Id,
            vehicleId = vehicle2Id,
            startUtc = TimeTestFixtures.Period2.Start_Invalid_ConflictWithPeriod1,
            endUtc = TimeTestFixtures.Period2.End_Valid
        };

        var response = await _client.PostAsJsonAsync("/assignments", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Should_return_201_when_assignments_are_back_to_back_for_the_same_driver_and_vehicle()
    {
        // Arrange
        var driver1Id = await _dbSeeder.SeedDriver("Driver1");
        var vehicle1Id = await _dbSeeder.SeedVehicle("Vehicle1");

        var assignment1 = new
        {
            driverId = driver1Id,
            vehicleId = vehicle1Id,
            startUtc = TimeTestFixtures.Period1.Start,
            endUtc = TimeTestFixtures.Period1.End_Valid
        };

        _ = await _client.PostAsJsonAsync("/assignments", assignment1);

        // Act
        var request = new
        {
            driverId = driver1Id,
            vehicleId = vehicle1Id,
            startUtc = TimeTestFixtures.Period2.Start_Valid_Back2BackWithPeriod1End,
            endUtc = TimeTestFixtures.Period2.End_Valid
        };

        var response = await _client.PostAsJsonAsync("/assignments", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Should_return_201_when_input_is_valid()
    {
        var driverId = await _dbSeeder.SeedDriver("Driver1");
        var vehicleId = await _dbSeeder.SeedVehicle("Vehicle1");

        var request = new
        {
            driverId,
            vehicleId,
            startUtc =  TimeTestFixtures.Period1.Start,
            endUtc = TimeTestFixtures.Period1.End_Valid
        };

        var response = await _client.PostAsJsonAsync("/assignments", request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}