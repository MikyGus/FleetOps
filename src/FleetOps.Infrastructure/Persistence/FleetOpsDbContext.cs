using Microsoft.EntityFrameworkCore;

namespace FleetOps.Infrastructure.Persistence;

public sealed class FleetOpsDbContext : DbContext
{
    public FleetOpsDbContext(DbContextOptions<FleetOpsDbContext> options) : base(options)
    {
    }
}
