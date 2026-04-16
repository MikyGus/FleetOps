using System.Net;
using System.Net.Http.Json;
using FleetOps.Tests.Integration.Infrastructure.Database;
using FleetOps.Tests.Integration.Infrastructure.Fixtures;
using Shouldly;
using Xunit.Abstractions;

namespace FleetOps.Tests.Integration.Application.Assignments;

public sealed class CreateAssignmentTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;
    private readonly TestDatabaseCleaner _databaseCleaner = new();

    private readonly (DateTimeOffset Start, DateTimeOffset End) _validTimeUtc = 
        (
            new DateTimeOffset(2026, 4, 5, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 4, 5, 11, 0, 0, TimeSpan.Zero)
        );

    public CreateAssignmentTests(
        IntegrationTestWebAppFactory factory,
        ITestOutputHelper output)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    public async Task InitializeAsync() 
        => await _databaseCleaner.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Should_return_400_when_driver_does_not_exist()
    {
        var request = new
        {
          driverId = Guid.NewGuid(),
          vehicleId = Guid.NewGuid(),
          startUtc =  _validTimeUtc.Start,
          endUtc = _validTimeUtc.End
        };

        var response = await _client.PostAsJsonAsync("/assignments", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_return_400_when_vehicle_does_not_exist()
    {
        var request = new
        {
            driverId = Guid.NewGuid(),
            vehicleId = Guid.NewGuid(),
            startUtc = _validTimeUtc.Start,
            endUtc = _validTimeUtc.End
        };

        var response = await _client.PostAsJsonAsync("/assignments", request);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}