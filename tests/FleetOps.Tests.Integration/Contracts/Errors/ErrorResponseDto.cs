using System.Text.Json.Serialization;

namespace FleetOps.Tests.Integration.Contracts.Errors;

public sealed record ErrorResponseDto(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("details")] Dictionary<string, ErrorDetailDto[]>? Details);