using FleetOps.Application.Assignments;
using FleetOps.Application.Assignments.GetAssignments;
using FleetOps.Application.Drivers;
using FleetOps.Application.Drivers.GetDrivers;
using FleetOps.Application.Validations;
using FleetOps.Application.Vehicles;
using FleetOps.Application.Vehicles.GetVehicles;
using FleetOps.Domain.Drivers;
using FleetOps.Domain.Vehicles;
using FleetOps.Infrastructure.Assignments;
using FleetOps.Infrastructure.Drivers;
using FleetOps.Infrastructure.Persistence;
using FleetOps.Infrastructure.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FleetOps.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Postgres");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'Postgres' is not configured.");
        }

        services.AddDbContext<FleetOpsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        services.AddScoped<IAssignmentQueries, AssignmentQueries>();

        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IDriverQueries, DriverQueries>();
        services.AddScoped<IEntityExistenceChecker<Driver, Guid>, DriverExistenceChecker>();

        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IVehicleQueries, VehicleQueries>();
        services.AddScoped<IEntityExistenceChecker<Vehicle, Guid>, VehicleExistenceChecker>();

        return services;
    }
}