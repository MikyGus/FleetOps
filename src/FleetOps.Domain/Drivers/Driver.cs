using FleetOps.Domain.Assignments;
using FleetOps.Domain.Errors;
using FleetOps.Domain.Exceptions;

namespace FleetOps.Domain.Drivers;

public sealed class Driver
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<Assignment> Assignments { get; private set; } = new List<Assignment>();

    private Driver() // For EF Core
    {
        Name = string.Empty;
    }

    public Driver(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException(nameof(name), ErrorCodes.Driver.Name.Required, "Name must be provided.");
        }

        Name = name.Trim();
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}