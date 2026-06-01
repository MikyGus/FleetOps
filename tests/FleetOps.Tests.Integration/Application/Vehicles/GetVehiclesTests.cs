using System.Net;
using System.Net.Http.Json;
using FleetOps.Api.Contracts;
using FleetOps.Domain.Errors;
using FleetOps.Tests.Integration.Contracts.Entities.Vehicles;
using FleetOps.Tests.Integration.Contracts.Errors;
using FleetOps.Tests.Integration.Infrastructure.Database;
using FleetOps.Tests.Integration.Infrastructure.Fixtures;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace FleetOps.Tests.Integration.Application.Vehicles;

public sealed class GetVehiclesTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DatabaseSeeder _seeder;

    public GetVehiclesTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _seeder = new DatabaseSeeder(factory.Services.GetRequiredService<IServiceScopeFactory>());
    }

    public async Task InitializeAsync() => await TestDatabaseCleaner.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Should_return_200_with_an_empty_list_when_the_database_is_empty()
    {
        var response = await _client.GetAsync("/vehicles");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<VehicleResponse>>();
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_return_400_when_registrationnumber_filter_is_too_long()
    {
        var url = QueryHelpers.AddQueryString("/vehicles", new Dictionary<string, string?>
        {
            ["registrationnumber"] = new string('X', 21)
        });
        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        error.ShouldNotBeNull();
        error.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);

        error.Details.ShouldNotBeNull();
        error.Details.Keys.ShouldBe(["RegistrationNumber"]);
        error.Details["RegistrationNumber"].ShouldContain(x => x.ErrorCode == ErrorCodes.Vehicle.RegistrationNumber.MaxLengthExceeded);
    }

    [Fact]
    public async Task Should_return_only_vehicles_matching_registration_number_filter()
    {
        var seedResult = await _seeder.Seed()
            .SeedVehicles(10, "Vehicle", true, DbSeedBuilder.AffixPosition.Postfix)
            .SeedVehicles(10, "VeHIcle", true, DbSeedBuilder.AffixPosition.Prefix)
            .SeedVehicles(40, "OtherTransports")
            .SeedVehicles(10, "VehiCLe", true, DbSeedBuilder.AffixPosition.Prefix | DbSeedBuilder.AffixPosition.Postfix)
            .SaveAsync();

        var url = QueryHelpers.AddQueryString("/vehicles", new Dictionary<string, string?>
        {
            ["registrationnumber"] = "vehicle"
        });
        var response = await _client.GetAsync(url);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<VehicleResponse>>();
        result.ShouldNotBeNull();
        result.Count.ShouldBe(30);

        var expectedVehicles = seedResult.Vehicles.Values
            .Where(x => x.RegistrationNumber
                .Contains("vehicle", StringComparison.InvariantCultureIgnoreCase))
            .OrderBy(x => x.RegistrationNumber)
            .Select(x => new VehicleResponse(x.Id, x.RegistrationNumber, x.IsActive))
            .ToList();
        result.ShouldBe(expectedVehicles);
    }

    [Fact]
    public async Task Should_return_the_correct_set_of_vehicles_when_using_limit()
    {
        var vehiclesToGenerate = 300;
        var limit = 30;
        var seedResult = await _seeder.Seed()
            .SeedVehicles(vehiclesToGenerate, "Vehicle")
            .SaveAsync();

        var url = QueryHelpers.AddQueryString("/vehicles", new Dictionary<string, string?>
        {
            ["limit"] = limit.ToString()
        });
        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<VehicleResponse>>();
        result.ShouldNotBeNull();
        result.Count.ShouldBe(limit);

        var expectedVehicles = seedResult.Vehicles.Values
            .OrderBy(x => x.RegistrationNumber)
            .Take(limit)
            .Select(x => new VehicleResponse(x.Id, x.RegistrationNumber, x.IsActive))
            .ToList();
        result.ShouldBe(expectedVehicles);
    }

    [Fact]
    public async Task Should_return_the_correct_set_of_vehicles_when_using_limit_with_offset()
    {
        var vehiclesToGenerate = 300;
        var limit = 30;
        var offset = 60;
        var seedResult = await _seeder.Seed()
            .SeedVehicles(vehiclesToGenerate, "Vehicle")
            .SaveAsync();

        var url = QueryHelpers.AddQueryString("/vehicles", new Dictionary<string, string?>
        {
            ["limit"] = limit.ToString(),
            ["offset"] = offset.ToString()
        });
        var response = await _client.GetAsync(url);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<VehicleResponse>>();
        result.ShouldNotBeNull();
        result.Count.ShouldBe(limit);

        var expectedVehicles = seedResult.Vehicles.Values
            .OrderBy(x => x.RegistrationNumber)
            .Skip(offset)
            .Take(limit)
            .Select(x => new VehicleResponse(x.Id, x.RegistrationNumber, x.IsActive))
            .ToList();
        result.ShouldBe(expectedVehicles);
    }
}