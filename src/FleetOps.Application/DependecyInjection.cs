using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using FleetOps.Application.Assignments.CreateAssignment;
using FleetOps.Application.Assignments.GetAssignments;

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

        return services;
    }
}
