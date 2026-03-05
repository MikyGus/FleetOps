using Microsoft.EntityFrameworkCore;

namespace FleetOps.Infrastructure.Persistence;

public sealed class FleetOpsDbContext : DbContext
{
    public FleetOpsDbContext(DbContextOptions<FleetOpsDbContext> options) : base(options)
    {
    }

    public DbSet<Assignment> Assignments => Set<Assignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
           entity.ToTable("assignments");

           entity.HasKey(x => x.Id);

           entity.Property(x => x.Id).HasColumnName("id");
           entity.Property(x => x.DriverId).HasColumnName("driver_id").IsRequired();
           entity.Property(x => x.VehicleId).HasColumnName("vehicle_id").IsRequired();

           entity.Property(x => x.StartUtc).HasColumnName("start_utc").IsRequired();
           entity.Property(x => x.EndUtc).HasColumnName("end_utc").IsRequired(); 
        });
    }
}
