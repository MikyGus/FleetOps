using Npgsql;

namespace FleetOps.Tests.Integration.Infrastructure.Database;

public sealed class TestDatabaseCleaner
{
    public async Task ResetAsync()
    {
        await using var connection = new NpgsqlConnection(TestConfiguration.PostgresConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();

        command.CommandText = """
            TRUNCATE TABLE assignments, drivers, vehicles RESTART IDENTITY CASCADE;
        """;

        await command.ExecuteNonQueryAsync();
    }
}