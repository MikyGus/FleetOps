namespace FleetOps.Domain.Drivers;

public sealed class Driver
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    private Driver() // For EF Core
    {
        Name = string.Empty;
    }

    public Driver(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name must be provided.", nameof(name));
        }

        Name = name.Trim();
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}