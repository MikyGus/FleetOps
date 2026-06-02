using System.Net.Http.Json;
using FleetOps.Api.Contracts;
using FleetOps.Domain.Errors;
using FleetOps.Tests.Integration.Contracts.Entities.Drivers;
using FleetOps.Tests.Integration.Contracts.Errors;
using FleetOps.Tests.Integration.Infrastructure.Database;
using FleetOps.Tests.Integration.Infrastructure.Fixtures;
using Shouldly;

namespace FleetOps.Tests.Integration.Application.Drivers;

public sealed class CreateDriverTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public CreateDriverTests(IntegrationTestWebAppFactory factory)
        => _client = factory.CreateClient();

    public async Task InitializeAsync() => await TestDatabaseCleaner.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Should_return_400_when_drivername_is_empty()
    {
        var request = new CreateDriverRequest("");

        var response = await _client.PostAsJsonAsync("/drivers", request);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var errorResult = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        errorResult.ShouldNotBeNull();
        errorResult.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);
        errorResult.Details.ShouldNotBeNull();
        errorResult.Details.Keys.ShouldBe(["Name"]);
        errorResult.Details["Name"].ShouldContain(x => x.ErrorCode == ErrorCodes.Driver.Name.Required);
    }

    [Fact]
    public async Task Should_return_400_when_drivername_is_too_long()
    {
        var request = new CreateDriverRequest(new string('X', 201));

        var response = await _client.PostAsJsonAsync("/drivers", request);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var errorResult = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        errorResult.ShouldNotBeNull();
        errorResult.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);
        errorResult.Details.ShouldNotBeNull();
        errorResult.Details.Keys.ShouldBe(["Name"]);
        errorResult.Details["Name"].ShouldContain(x => x.ErrorCode == ErrorCodes.Driver.Name.MaxLengthExceeded);
    }

    [Fact]
    public async Task Should_return_201_when_drivername_contains_value_but_not_too_long()
    {
        var name = new string('X', 200);
        var request = new CreateDriverRequest(name);

        var response = await _client.PostAsJsonAsync("/drivers", request);

        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Created);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var created = await response.Content.ReadFromJsonAsync<CreateDriverResponse>();

        created.ShouldNotBeNull();
        created.Id.ShouldNotBe(Guid.Empty);

        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location!.AbsolutePath.ShouldBe($"/drivers/{created.Id}");

        var driver = await _client.GetFromJsonAsync<DriverResponse>(response.Headers.Location.AbsolutePath);
        driver.ShouldNotBeNull();
        driver.Id.ShouldBe(created.Id);
        driver.Name.ShouldBe(name);
        driver.IsActive.ShouldBeTrue();
    }
}