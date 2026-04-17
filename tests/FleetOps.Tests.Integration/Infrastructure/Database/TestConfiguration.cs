using Microsoft.Extensions.Configuration;

namespace FleetOps.Tests.Integration.Infrastructure.Database;

public static class TestConfiguration
{
    private static readonly IConfigurationRoot Configuration = 
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Testing.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

    public static string PostgresConnectionString =>
        Configuration.GetConnectionString("Postgres")
        ?? throw new InvalidOperationException(
            "Missing connection string 'ConnectionStrings:Postgres' for integration tests.");
}