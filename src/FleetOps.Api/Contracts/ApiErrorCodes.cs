namespace FleetOps.Api.Contracts;

public static class ApiErrorCodes
{
    public static (string ErrorCode, string Message) ValidationError => ("validation_error", "One or more validation errors occurred.");
    public static (string ErrorCode, string Message) ServerError => ("server_error", "An unexpected error occurred.");
}