using System.Net;
using System.Net.Http.Json;
using FleetOps.Tests.Integration.Infrastructure.Fixtures;

namespace FleetOps.Tests.Integration.Application.Assignments;

public sealed class CreateDriverTests : IClassFixture<IntegrationTestWebAppFactory>
{
    private readonly HttpClient _client;

    public CreateDriverTests(IntegrationTestWebAppFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_return_400_when_driver_name_is_empty()
    {
        var request = new
        {
            name = ""
        };

        var response = await _client.PostAsJsonAsync("/drivers", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}