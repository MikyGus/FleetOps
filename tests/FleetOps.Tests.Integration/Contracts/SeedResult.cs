namespace FleetOps.Tests.Integration.Contracts;

public sealed class SeedResult
{
    public Dictionary<string, Guid> Drivers { get; } = [];
    public Dictionary<string, Guid> Vehicles { get; } = [];
    public Dictionary<string, Guid> Assignments { get; } = [];
}