using System.Net;
using System.Net.Http.Json;
using FleetOps.Api.Contracts;
using FleetOps.Domain.Errors;
using FleetOps.Tests.Integration.Contracts.Entities.Vehicles;
using FleetOps.Tests.Integration.Contracts.Errors;
using FleetOps.Tests.Integration.Infrastructure.Database;
using FleetOps.Tests.Integration.Infrastructure.Fixtures;
using Shouldly;

namespace FleetOps.Tests.Integration.Application.Vehicles;

public sealed class CreateVehicleTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public CreateVehicleTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync() => await TestDatabaseCleaner.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Should_return_400_when_registration_number_is_empty()
    {
        var request = new CreateVehicleRequest("");

        var response = await _client.PostAsJsonAsync("/vehicles", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        result.ShouldNotBeNull();
        result.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);

        result.Details.ShouldNotBeNull();
        result.Details.Keys.ShouldBe(["RegistrationNumber"]);
        result.Details["RegistrationNumber"].ShouldContain(x => x.ErrorCode == ErrorCodes.Vehicle.RegistrationNumber.Required);
    }

    [Fact]
    public async Task Should_return_400_when_registration_number_is_too_long()
    {
        var request = new CreateVehicleRequest(new string('X', 21));

        var response = await _client.PostAsJsonAsync("/vehicles", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        result.ShouldNotBeNull();
        result.Code.ShouldBe(ApiErrorCodes.ValidationError.ErrorCode);

        result.Details.ShouldNotBeNull();
        result.Details.Keys.ShouldBe(["RegistrationNumber"]);
        result.Details["RegistrationNumber"].ShouldContain(x => x.ErrorCode == ErrorCodes.Vehicle.RegistrationNumber.MaxLengthExceeded);
    }

    [Fact]
    public async Task Should_return_201_when_registration_number_contains_value_but_not_too_long()
    {
        var registrationNumber = new string('X', 20);
        var request = new CreateVehicleRequest(registrationNumber);

        var response = await _client.PostAsJsonAsync("/vehicles", request);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var result = await response.Content.ReadFromJsonAsync<CreateVehicleResponse>();

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);

        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location!.AbsolutePath.ShouldBe($"/vehicles/{result.Id}");

        var vehicle = await _client.GetFromJsonAsync<VehicleResponse>(response.Headers.Location.AbsolutePath);
        vehicle.ShouldNotBeNull();
        vehicle.Id.ShouldBe(result.Id);
        vehicle.RegistrationNumber.ShouldBe(registrationNumber);
        vehicle.IsActive.ShouldBeTrue();
    }
}