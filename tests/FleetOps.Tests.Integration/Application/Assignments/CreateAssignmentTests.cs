using System.Net;
using System.Net.Http.Json;
using FleetOps.Api.Contracts;
using FleetOps.Domain.Errors;
using FleetOps.Tests.Integration.Contracts.Errors;
using FleetOps.Tests.Integration.Infrastructure.Database;
using FleetOps.Tests.Integration.Infrastructure.Fixtures;
using FleetOps.Tests.Integration.Infrastructure.Scenarios;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace FleetOps.Tests.Integration.Application.Assignments;

public sealed class CreateAssignmentTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private DatabaseSeeder _dbSeeder;

    public CreateAssignmentTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
       _dbSeeder = new DatabaseSeeder(factory.Services.GetRequiredService<IServiceScopeFactory>());
    }

    public async Task InitializeAsync() => await TestDatabaseCleaner.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Should_return_400_when_driver_does_not_exist()
    {
        var seedResult = await _dbSeeder.Seed().SeedVehicle("Vehicle", true).SaveAsync();

        var request = AssignmentRequestBuilder.WithMissingDriver(seedResult.Vehicles["Vehicle"]).Build();

        var response = await _client.PostAsJsonAsync("/assignments", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldStartWith("application/json");

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        error.ShouldNotBeNull();
        error.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);
        error.Details.ShouldNotBeNull();
        error.Details.ShouldContainKey("DriverId");
        error.Details["DriverId"].ShouldContain(detail => detail.ErrorCode == ErrorCodes.Assignment.DriverId.NotFound);
    }

    [Fact]
    public async Task Should_return_400_when_vehicle_does_not_exist()
    {
        var seedResult = await _dbSeeder.Seed().SeedDriver("Driver").SaveAsync();

        var request = AssignmentRequestBuilder.WithMissingVehicle(seedResult.Drivers["Driver"]).Build();

        var response = await _client.PostAsJsonAsync("/assignments", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldStartWith("application/json");

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        error.ShouldNotBeNull();
        error.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);
        error.Details.ShouldNotBeNull();
        error.Details.ShouldContainKey("VehicleId");
        error.Details["VehicleId"].ShouldContain(detail => detail.ErrorCode == ErrorCodes.Assignment.VehicleId.NotFound);
    }

    [Fact]
    public async Task Should_return_400_when_endtime_is_before_starttime()
    {
        var seedResult = await _dbSeeder.Seed()
            .SeedDriver("Driver")
            .SeedVehicle("Vehicle", true)
            .SaveAsync();

        var request = AssignmentRequestBuilder
            .For(seedResult.Drivers["Driver"], seedResult.Vehicles["Vehicle"])
            .WithEndBeforeStart()
            .Build();

        var response = await _client.PostAsJsonAsync("/assignments", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldStartWith("application/json");

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        error.ShouldNotBeNull();
        error.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);
        error.Details.ShouldNotBeNull();
        error.Details.ShouldContainKey("StartUtc");
        error.Details["StartUtc"].ShouldContain(detail => detail.ErrorCode == ErrorCodes.Assignment.TimeRange.Invalid);
        error.Details.ShouldContainKey("EndUtc");
        error.Details["EndUtc"].ShouldContain(detail => detail.ErrorCode == ErrorCodes.Assignment.TimeRange.Invalid);
    }

    [Fact]
    public async Task Should_return_409_when_driver_has_overlapping_assignments()
    {
        // Arrange
        var seedResult = await _dbSeeder.Seed()
            .SeedAssignment("Driver1", "Vehicle1", TimeTestFixtures.Period1.Start, TimeTestFixtures.Period1.End_Valid)
            .SeedVehicle("Vehicle2")
            .SaveAsync();

        // Act
        var request = AssignmentRequestBuilder
            .For(seedResult.Drivers["Driver1"], seedResult.Vehicles["Vehicle2"])
            .OverlappingPeriod1().Build();
        var response = await _client.PostAsJsonAsync("/assignments", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        response.Content.Headers.ContentType?.MediaType.ShouldStartWith("application/json");

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        error.ShouldNotBeNull();
        error.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);
        error.Details.ShouldNotBeNull();
        error.Details.ShouldContainKey("DriverId");
        error.Details["DriverId"].ShouldContain(detail => detail.ErrorCode == ErrorCodes.Assignment.DriverId.Overlap);        
    }

    [Fact]
    public async Task Should_return_409_when_vehicle_has_overlapping_assignments()
    {
        // Arrange
        var seedResult = await _dbSeeder.Seed()
            .SeedAssignment("Driver1", "Vehicle1", TimeTestFixtures.Period1.Start, TimeTestFixtures.Period1.End_Valid)
            .SeedDriver("Driver2")
            .SaveAsync();

        // Act
        var request = AssignmentRequestBuilder
            .For(seedResult.Drivers["Driver2"], seedResult.Vehicles["Vehicle1"]).OverlappingPeriod1().Build();

        var response = await _client.PostAsJsonAsync("/assignments", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        response.Content.Headers.ContentType?.MediaType.ShouldStartWith("application/json");

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        error.ShouldNotBeNull();
        error.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);
        error.Details.ShouldNotBeNull();
        error.Details.ShouldContainKey("VehicleId");
        error.Details["VehicleId"].ShouldContain(detail => detail.ErrorCode == ErrorCodes.Assignment.VehicleId.Overlap);    
    }

    [Fact]
    public async Task Should_return_201_when_assignments_time_overlaps_but_driver_and_vehicle_are_different()
    {
        // Arrange
        var seedResult = await _dbSeeder.Seed()
            .SeedAssignment("Driver1", "Vehicle1", TimeTestFixtures.Period1.Start, TimeTestFixtures.Period1.End_Valid)
            .SeedDriver("Driver2")
            .SeedVehicle("Vehicle2")
            .SaveAsync();

        // Act
        var request = AssignmentRequestBuilder
            .For(seedResult.Drivers["Driver2"], seedResult.Vehicles["Vehicle2"])
            .OverlappingPeriod1()
            .Build();

        var response = await _client.PostAsJsonAsync("/assignments", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Should_return_201_when_assignments_are_back_to_back_for_the_same_driver_and_vehicle()
    {
        // Arrange
        var seedResult = await _dbSeeder.Seed()
            .SeedAssignment("Driver1", "Vehicle1", TimeTestFixtures.Period1.Start, TimeTestFixtures.Period1.End_Valid)
            .SaveAsync();

        // Act
        var request = AssignmentRequestBuilder
            .For(seedResult.Drivers["Driver1"], seedResult.Vehicles["Vehicle1"]).BackToBackAfterPeriod1().Build();

        var response = await _client.PostAsJsonAsync("/assignments", request);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Should_return_201_when_input_is_valid()
    {
        var seedResult = await _dbSeeder.Seed()
            .SeedDriver("Driver1")
            .SeedVehicle("Vehicle1")
            .SaveAsync();

        var request = AssignmentRequestBuilder.For(seedResult.Drivers["Driver1"], seedResult.Vehicles["Vehicle1"]).Build();

        var response = await _client.PostAsJsonAsync("/assignments", request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}