using FleetOps.Tests.Integration.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FleetOps.Tests.Integration.Infrastructure.Fixtures;

public sealed class IntegrationTestWebAppFactory : WebApplicationFactory<Program>
{
    public IntegrationTestWebAppFactory()
    {
        Environment.SetEnvironmentVariable(
            "ConnectionStrings__Postgres",
            TestConfiguration.PostgresConnectionString);

        Environment.SetEnvironmentVariable(
            "ASPNETCORE_ENVIROMENT",
            "Testing");
    }
}