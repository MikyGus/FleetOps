using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using FleetOps.Application.Assignments.CreateAssignment;
using FleetOps.Application.Assignments.GetAssignments;
using FleetOps.Application.Drivers.CreateDriver;
using FleetOps.Application.Drivers.GetDrivers;
using FleetOps.Application.Vehicles.CreateVehicle;
using FleetOps.Application.Vehicles.GetVehicles;

namespace FleetOps.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateAssignmentHandler>();
        services.AddScoped<GetAssignmentsHandler>();
        services.AddScoped<GetAssignmentByIdHandler>();

        services.AddScoped<IValidator<CreateAssignmentCommand>, CreateAssignmentCommandValidator>();
        services.AddScoped<IValidator<GetAssignmentsQuery>, GetAssignmentsQueryValidator>();

        services.AddScoped<CreateDriverHandler>();
        services.AddScoped<GetDriversHandler>();
        services.AddScoped<GetDriverByIdHandler>();

        services.AddScoped<IValidator<CreateDriverCommand>, CreateDriverCommandValidator>();
        services.AddScoped<IValidator<GetDriversQuery>, GetDriversQueryValidator>();

        services.AddScoped<CreateVehicleHandler>();
        services.AddScoped<GetVehicleByIdHandler>();

        services.AddScoped<IValidator<CreateVehicleCommand>, CreateVehicleCommandValidator>();

        return services;
    }
}
