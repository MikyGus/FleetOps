using FleetOps.Api.Contracts;
using FleetOps.Domain.Errors;
using FleetOps.Domain.Exceptions;
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
        catch (DomainValidationException ex)
        {
            await HandleDomainValidationException(context, ex);
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx)
        {
            await HandlePostgresException(context, pgEx);
        }
        catch (Exception)
        {
            await HandleUnexpectedException(context);
        }
    }

    private async Task HandleDomainValidationException(HttpContext context, DomainValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse(
            ApiErrorCodes.ValidationError.ErrorCode,
            ApiErrorCodes.ValidationError.Message,
            new ()
            {
                { 
                    ex.PropertyName, 
                    [ new ErrorDetail(ex.ErrorCode, ex.Message) ] 
                }
            });

        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task HandleValidationException(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        Dictionary<string, ErrorDetail[]> details = ex.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => new ErrorDetail(
                    x.ErrorCode,
                    x.ErrorMessage)).ToArray());

        var response = new ErrorResponse(
            ApiErrorCodes.ValidationError.ErrorCode,
            ApiErrorCodes.ValidationError.Message,
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
                    ApiErrorCodes.ValidationError.ErrorCode,
                    ApiErrorCodes.ValidationError.Message,
                    new ()
                    {
                        { "DriverId", 
                        [new ErrorDetail(
                            ErrorCodes.Assignment.DriverId.Overlap,
                            "Driver already has an assignment during this time period.")] 
                        }
                    });
                break;

            case "ex_assignments_vehicle_no_overlap":
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = new ErrorResponse(
                    ApiErrorCodes.ValidationError.ErrorCode,
                    ApiErrorCodes.ValidationError.Message,
                    new ()
                    {
                        { "VehicleId", 
                        [new ErrorDetail(
                            ErrorCodes.Assignment.VehicleId.Overlap,
                            "Vehicle already has an assignment during this time period.")] 
                        }
                    });                
                break;

            case "ck_assignments_time":
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                response = new ErrorResponse(
                    ApiErrorCodes.ValidationError.ErrorCode,
                    ApiErrorCodes.ValidationError.Message,
                    new ()
                    {
                        { 
                            "startUtc", [new ErrorDetail(ErrorCodes.Assignment.TimeRange.Invalid,"StartUtc must be earlier than EndUtc.")]
                        },
                        {
                            "endUtc", [new ErrorDetail(ErrorCodes.Assignment.TimeRange.Invalid,"EndUtc must be later than StartUtc.")] 
                        }
                    });      
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                response = new ErrorResponse(
                    ApiErrorCodes.ServerError.ErrorCode,
                    ApiErrorCodes.ServerError.Message,
                    new ()
                    {
                        { ApiErrorCodes.ServerError.ErrorCode, 
                        [new ErrorDetail(
                            ApiErrorCodes.ServerError.ErrorCode,
                            ApiErrorCodes.ServerError.Message)] 
                        }
                    }); 
                break;
        }

        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task HandleUnexpectedException(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse(
            ApiErrorCodes.ServerError.ErrorCode,
            ApiErrorCodes.ServerError.Message,
            new ()
            {
                { ApiErrorCodes.ServerError.ErrorCode, 
                [new ErrorDetail(
                    ApiErrorCodes.ServerError.ErrorCode,
                    ApiErrorCodes.ServerError.Message)] 
                }
            }); 

        await context.Response.WriteAsJsonAsync(response);
    }
}