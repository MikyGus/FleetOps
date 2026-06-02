using FleetOps.Domain.Assignments;
using FleetOps.Domain.Drivers;
using FleetOps.Domain.Vehicles;

namespace FleetOps.Tests.Integration.Contracts;

public sealed class SeedResult
{
    public Dictionary<string, Driver> Drivers { get; } = [];
    public Dictionary<string, Vehicle> Vehicles { get; } = [];
    public Dictionary<string, Assignment> Assignments { get; } = [];
}
