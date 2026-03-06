namespace FleetOps.Api.Contracts;

public sealed record ErrorResponse(
    string Code,
    string Message,
    Dictionary<string, string[]>? Details = null
);