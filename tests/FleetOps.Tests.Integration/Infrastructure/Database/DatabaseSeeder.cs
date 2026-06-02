using Microsoft.Extensions.DependencyInjection;

namespace FleetOps.Tests.Integration.Infrastructure.Database;

public sealed class DatabaseSeeder
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DatabaseSeeder(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public DbSeedBuilder Seed() => new(_scopeFactory);
}
