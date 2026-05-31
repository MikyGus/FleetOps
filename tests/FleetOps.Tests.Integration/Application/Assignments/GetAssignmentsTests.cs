using System.Net;
using System.Net.Http.Json;
using FleetOps.Api.Contracts;
using FleetOps.Domain.Errors;
using FleetOps.Tests.Integration.Contracts.Assignments;
using FleetOps.Tests.Integration.Infrastructure.Database;
using FleetOps.Tests.Integration.Infrastructure.Fixtures;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

public sealed class GetAssignmentsTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DatabaseSeeder _seeder;

    public GetAssignmentsTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _seeder = new DatabaseSeeder(factory.Services.GetRequiredService<IServiceScopeFactory>());
    }

    public async Task InitializeAsync() => await TestDatabaseCleaner.ResetAsync();
    public async Task DisposeAsync() => await Task.CompletedTask;

    [Fact]
    public async Task Should_return_200_with_an_empty_list_when_database_is_empty()
    {
        var result = await _client.GetFromJsonAsync<List<AssignmentResponse>>("/assignments");

        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_return_only_assignments_for_driver()
    {
        var seedResult = await _seeder.Seed()
            .SeedAssignments(10,"Driver","Vehicle")
            .SeedAssignments(5, "Driver2", "Vehicle3")
            .SeedAssignments(5, "Driver3", "Vehicle4")
            // We add "Driver" again but with 10 months earlier to mix it up a bit
            .SeedAssignments(5,"Driver","Vehicle3",TimeTestFixtures.Period1.Start.AddMonths(-10), TimeTestFixtures.Period1.End_Valid.AddMonths(-10))
            .SaveAsync();

        var driver = seedResult.Drivers["Driver"];
        var vehicle = seedResult.Vehicles["Vehicle"];
        var vehicle3 = seedResult.Vehicles["Vehicle3"];

        var url = QueryHelpers.AddQueryString("/assignments", new Dictionary<string, string?>
        {
            ["driverId"] = driver.Id.ToString()
        });

        var response = await _client.GetAsync(url);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<AssignmentResponse>>();
        result.ShouldNotBeNull();
        result.Count.ShouldBe(15);

        result.ShouldAllBe(x => x.DriverId == driver.Id);

        result.Count(x => x.VehicleId == vehicle.Id).ShouldBe(10);
        result.Count(x => x.VehicleId == vehicle3.Id).ShouldBe(5);

        var expectedAssignments = seedResult.Assignments.Values
            .Where(x => x.DriverId == driver.Id)
            .OrderBy(x => x.StartUtc)
            .Select(x => new AssignmentResponse(x.Id, x.DriverId, x.VehicleId, x.StartUtc, x.EndUtc))
            .ToList();
        result.ShouldBe(expectedAssignments);
    }

    [Fact]
    public async Task Should_return_only_assignments_for_vehicle()
    {
        var seedResult = await _seeder.Seed()
            .SeedAssignments(5, "Driver", "Vehicle")
            .SeedAssignments(5, "Driver3", "Vehicle3")
            .SeedAssignments(8, "Driver2", "Vehicle", TimeTestFixtures.Period1.Start.AddMonths(-5), TimeTestFixtures.Period1.End_Valid.AddMonths(-5))
            .SaveAsync();

        var driver = seedResult.Drivers["Driver"];
        var driver2 = seedResult.Drivers["Driver2"];
        var vehicle = seedResult.Vehicles["Vehicle"];

        var url = QueryHelpers.AddQueryString("/assignments", new Dictionary<string, string?>
        {
            ["vehicleId"] = vehicle.Id.ToString()
        });

        var response = await _client.GetAsync(url);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<AssignmentResponse>>();
        
        result.ShouldNotBeNull();
        result.Count.ShouldBe(13);

        result.Count(x => x.DriverId == driver.Id).ShouldBe(5);
        result.Count(x => x.DriverId == driver2.Id).ShouldBe(8);

        var expectedAssignments = seedResult.Assignments.Values
            .Where(x => x.VehicleId == vehicle.Id)
            .OrderBy(x => x.StartUtc)
            .Select(x => new AssignmentResponse(x.Id, x.DriverId, x.VehicleId, x.StartUtc, x.EndUtc))
            .ToList();
        result.ShouldBe(expectedAssignments);
    }

    [Fact]
    public async Task Should_return_only_assignments_within_datetime_range()
    {
        var assignmentsToGenerate = 378;
        var seedResult = await _seeder.Seed()
            .SeedAssignments(assignmentsToGenerate, "Driver", "Vehicle")
            .SaveAsync();

        var fromUtc = TimeTestFixtures.Period1.Start.AddHours(1);
        var toUtc = TimeTestFixtures.Period1.End_Valid.AddHours(10);

        var url = QueryHelpers.AddQueryString("/assignments", new Dictionary<string, string?>
        {
           ["fromUtc"] = fromUtc.ToUniversalTime().ToString("O"),
           ["toUtc"] = toUtc.ToUniversalTime().ToString("O"),
           ["limit"] = assignmentsToGenerate.ToString()
        });

        var response = await _client.GetAsync(url);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<AssignmentResponse>>();

        result.ShouldNotBeNull();
        
        var expectedAssignments = seedResult.Assignments.Values
            .Where(x => x.EndUtc > fromUtc && x.StartUtc < toUtc)
            .OrderBy(x => x.StartUtc)
            .Select(x => new AssignmentResponse(x.Id, x.DriverId, x.VehicleId, x.StartUtc, x.EndUtc))
            .ToList();
        result.ShouldBe(expectedAssignments);
    }

    [Fact]
    public async Task Should_return_the_correct_amount_of_assignments_when_using_limit()
    {
        var assignmentsToGenerate = 300;
        var limit = 30;
        var seedResult = await _seeder.Seed()
            .SeedAssignments(assignmentsToGenerate, "Driver", "Vehicle")
            .SaveAsync();
        
        var url = QueryHelpers.AddQueryString("/assignments", new Dictionary<string, string?>
        {
           ["limit"] = limit.ToString() 
        });
        var response = await _client.GetAsync(url);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<AssignmentResponse>>();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(limit);

        var expectedAssignments = seedResult.Assignments.Values
            .OrderBy(x => x.StartUtc)
            .Take(limit)
            .Select(x => new AssignmentResponse(x.Id, x.DriverId, x.VehicleId, x.StartUtc, x.EndUtc))
            .ToList();
        result.ShouldBe(expectedAssignments);
    }

    [Fact]
    public async Task Should_return_the_correct_amount_of_assignments_and_page_when_using_limit_and_offset()
    {
        var assignmentsToGenerate = 300;
        var limit = 30;
        var offset = 100;
        var seedResult = await _seeder.Seed()
            .SeedAssignments(assignmentsToGenerate, "Driver", "Vehicle")
            .SaveAsync();
        
        var url = QueryHelpers.AddQueryString("/assignments", new Dictionary<string, string?>
        {
           ["limit"] = limit.ToString(),
           ["offset"] = offset.ToString()
        });
        var response = await _client.GetAsync(url);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<AssignmentResponse>>();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(limit);

        var expectedAssignments = seedResult.Assignments.Values
            .OrderBy(x => x.StartUtc)
            .Skip(offset)
            .Take(limit)
            .Select(x => new AssignmentResponse(x.Id, x.DriverId, x.VehicleId, x.StartUtc, x.EndUtc))
            .ToList();
        result.ShouldBe(expectedAssignments);
    }

    [Fact]
    public async Task Should_return_400_when_endtime_is_before_starttime()
    {
        var fromUtc = TimeTestFixtures.Period1.End_Valid;
        var toUtc = TimeTestFixtures.Period1.Start;

        var url = QueryHelpers.AddQueryString("/assignments", new Dictionary<string, string?>
        {
           ["fromUtc"] = fromUtc.ToUniversalTime().ToString("O"),
           ["toUtc"] = toUtc.ToUniversalTime().ToString("O")
        });
        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        error.ShouldNotBeNull();
        error.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);

        error.Details.ShouldNotBeNull();
        error.Details.Keys.ShouldBe(["FromUtc", "ToUtc"], ignoreOrder: true);
        error.Details["FromUtc"].ShouldContain(x => x.ErrorCode == ErrorCodes.Assignment.TimeRange.Invalid);
        error.Details["ToUtc"].ShouldContain(x => x.ErrorCode == ErrorCodes.Assignment.TimeRange.Invalid);
    }
}