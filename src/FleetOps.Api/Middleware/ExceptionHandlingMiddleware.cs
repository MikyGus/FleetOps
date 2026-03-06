using FleetOps.Api.Contracts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FleetOps.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await HandleValidationException(context, ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            await HandlePostgresException(context, pgEx);
        }
    }

    private static async Task HandleValidationException(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        Dictionary<string, string[]> details = ex.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.ErrorMessage).ToArray());

        var response = new ErrorResponse(
            "validation_error",
            "One or more validation errors occurred.",
            details);

        await context.Response.WriteAsJsonAsync(response);
    }

    private async Task HandlePostgresException(HttpContext context, PostgresException pgEx)
    {
        context.Response.ContentType = "application/json";

        ErrorResponse response;

        switch (pgEx.ConstraintName)
        {
            case "ex_assignments_driver_no_overlap":
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = new ErrorResponse(
                    "driver_overlap",
                    "Driver already has an assignment during this time period.");
                break;

            case "ex_assignments_vehicle_no_overlap":
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = new ErrorResponse(
                    "vehicle_overlap",
                    "Vehicle already has an assignment during this time period.");
                break;

            case "ck_assignments_time":
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ErrorResponse(
                    "invalid_time_range",
                    "EndUtc must be greater than StartUtc");
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new ErrorResponse(
                    "database_error",
                    "An unexpected database error occurred.");
                break;
        }

        await context.Response.WriteAsJsonAsync(response);
    }
}