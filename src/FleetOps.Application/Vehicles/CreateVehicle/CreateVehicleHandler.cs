using FluentValidation;
using FleetOps.Domain.Vehicles;

namespace FleetOps.Application.Vehicles.CreateVehicle;

public sealed class CreateVehicleHandler
{
    private readonly IVehicleRepository _repository;

    private readonly IValidator<CreateVehicleCommand> _validator;
    public CreateVehicleHandler(
        IVehicleRepository repository,
        IValidator<CreateVehicleCommand> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<CreateVehicleResult> HandleAsync(CreateVehicleCommand command, CancellationToken ct)
    {
        await _validator.ValidateAndThrowAsync(command, ct);

        var vehicle = new Vehicle(command.RegistrationNumber);

        await _repository.AddAsync(vehicle, ct);
        await _repository.SaveChangesAsync(ct);

        return new CreateVehicleResult(vehicle.Id);
    }

}
