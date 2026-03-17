using System.IO.Compression;

namespace FleetOps.Domain.Vehicles;

public sealed class Vehicle
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string RegistrationNumber { get; private set; }
    public bool IsActive { get; private set; }

    private Vehicle()  // For EF Core
    {
        RegistrationNumber = string.Empty;
    }

    public Vehicle(string registrationnumber)
    {
        if (string.IsNullOrWhiteSpace(registrationnumber))
        {
            throw new ArgumentException("RegistrationNumber must be provided.", nameof(registrationnumber));
        }

        RegistrationNumber = registrationnumber.Trim().ToUpperInvariant();
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}