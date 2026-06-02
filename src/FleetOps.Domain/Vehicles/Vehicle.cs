using FleetOps.Domain.Assignments;
using FleetOps.Domain.Errors;
using FleetOps.Domain.Exceptions;

namespace FleetOps.Domain.Vehicles;

public sealed class Vehicle
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string RegistrationNumber { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<Assignment> Assignments { get; private set; } = new List<Assignment>();

    private Vehicle()  // For EF Core
    {
        RegistrationNumber = string.Empty;
    }

    public Vehicle(string registrationnumber)
    {
        if (string.IsNullOrWhiteSpace(registrationnumber))
        {
            throw new DomainValidationException(nameof(registrationnumber), ErrorCodes.Vehicle.RegistrationNumber.Required, "RegistrationNumber must be provided.");
        }

        RegistrationNumber = registrationnumber.Trim().ToUpperInvariant();
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
