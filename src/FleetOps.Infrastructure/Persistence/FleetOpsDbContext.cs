using FleetOps.Domain.Assignments;
using FleetOps.Domain.Drivers;
using FleetOps.Domain.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace FleetOps.Infrastructure.Persistence;

public sealed class FleetOpsDbContext : DbContext
{
    public FleetOpsDbContext(DbContextOptions<FleetOpsDbContext> options) : base(options)
    {
    }

    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();

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

        modelBuilder.Entity<Driver>(entity =>
        {
           entity.ToTable("drivers");

           entity.HasKey(x => x.Id);

           entity.Property(x => x.Id)
                .HasColumnName("id");
           
           entity.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();
            
           entity.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .IsRequired();
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
           entity.ToTable("vehicles");

           entity.HasKey(x => x.Id);

           entity.Property(x => x.Id).HasColumnName("id");

           entity.Property(x => x.RegistrationNumber)
                .HasColumnName("registration_number")
                .HasMaxLength(20)
                .IsRequired();
                
           entity.Property(x => x.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

           entity.HasIndex(x => x.RegistrationNumber).IsUnique();
        });
    }
}
