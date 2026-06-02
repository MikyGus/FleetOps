using System.Text.Json.Serialization;

namespace FleetOps.Tests.Integration.Contracts.Errors;

public sealed record ErrorDetailDto(
    [property: JsonPropertyName("errorCode")] string ErrorCode,
    [property: JsonPropertyName("message")] string Message);