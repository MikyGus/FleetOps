using System.Net.Http.Json;
using FleetOps.Api.Contracts;
using FleetOps.Domain.Errors;
using FleetOps.Tests.Integration.Contracts.Entities.Drivers;
using FleetOps.Tests.Integration.Infrastructure.Database;
using FleetOps.Tests.Integration.Infrastructure.Fixtures;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace FleetOps.Tests.Integration.Application.Drivers;

public sealed class GetDriversTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DatabaseSeeder _seeder;

    public GetDriversTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
        _seeder = new DatabaseSeeder(factory.Services.GetRequiredService<IServiceScopeFactory>());
    }

    public async Task InitializeAsync() => await TestDatabaseCleaner.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Should_return_200_with_an_empty_list_when_database_is_empty()
    {
        var response = await _client.GetAsync("/drivers");
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<DriverResponse>>();
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0);
    }

    [Fact]
    public async Task Should_return_400_when_name_filter_is_too_long()
    {
        var url = QueryHelpers.AddQueryString("/drivers", new Dictionary<string, string?>
        {
           ["name"] = new string('X', 201) 
        });
        var response = await _client.GetAsync(url);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        error.ShouldNotBeNull();
        error.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);

        error.Details.ShouldNotBeNull();
        error.Details.Keys.ShouldBe(["Name"]);
        error.Details["Name"].ShouldContain(x => x.ErrorCode == ErrorCodes.Driver.Name.MaxLengthExceeded);
    }

    [Fact]
    public async Task Should_return_only_drivers_containing_searchstring()
    {
        var seedResult = await _seeder.Seed()
            .SeedDrivers(10, "Driver", DbSeedBuilder.AffixPosition.Postfix)
            .SeedDrivers(10, "DrIVer", DbSeedBuilder.AffixPosition.Prefix)
            .SeedDrivers(10, "DrivER", DbSeedBuilder.AffixPosition.Prefix | DbSeedBuilder.AffixPosition.Postfix)
            .SeedDrivers(40, "SomeoneElse")
            .SaveAsync();

        var url = QueryHelpers.AddQueryString("/drivers", new Dictionary<string, string?>
        {
           ["name"] = "driver" 
        });
        var response = await _client.GetAsync(url);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<DriverResponse>>();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(30);

        var expectedDrivers = seedResult.Drivers.Values
            .Where(x => x.Name.Contains("driver", StringComparison.InvariantCultureIgnoreCase))
            .OrderBy(x => x.Name)
            .Select(x => new DriverResponse(x.Id, x.Name, x.IsActive))
            .ToList();
        result.ShouldBe(expectedDrivers);
    }

    [Fact]
    public async Task Should_return_the_correct_set_of_drivers_when_using_limit()
    {
        var driversToGenerate = 300;
        var limit = 30;
        var seedResult = await _seeder.Seed()
            .SeedDrivers(driversToGenerate, "Driver")
            .SaveAsync();

        var url = QueryHelpers.AddQueryString("/drivers", new Dictionary<string, string?>
        {
           ["limit"] = limit.ToString() 
        });
        var response = await _client.GetAsync(url);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<DriverResponse>>();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(limit);

        var expectedDrivers = seedResult.Drivers.Values
            .OrderBy(x => x.Name)
            .Take(limit)
            .Select(x => new DriverResponse(x.Id, x.Name, x.IsActive))
            .ToList();
        result.ShouldBe(expectedDrivers);
    }

    [Fact]
    public async Task Should_return_the_correct_set_of_drivers_when_using_limit_with_offset()
    {
        var driversToGenerate = 300;
        var limit = 30;
        var offset = 50;
        var seedResult = await _seeder.Seed()
            .SeedDrivers(driversToGenerate, "Driver")
            .SaveAsync();

        var url = QueryHelpers.AddQueryString("/drivers", new Dictionary<string, string?>
        {
           ["limit"] = limit.ToString(),
           ["offset"] = offset.ToString()
        });
        var response = await _client.GetAsync(url);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<List<DriverResponse>>();

        result.ShouldNotBeNull();
        result.Count.ShouldBe(limit);

        var expectedDrivers = seedResult.Drivers.Values
            .OrderBy(x => x.Name)
            .Skip(offset)
            .Take(limit)
            .Select(x => new DriverResponse(x.Id, x.Name, x.IsActive))
            .ToList();
        result.ShouldBe(expectedDrivers);
    }
}