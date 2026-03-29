using FleetOps.Application.Validations;
using FluentValidation;

namespace FleetOps.Application.Vehicles.CreateVehicle;

public sealed class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(x => x.RegistrationNumber).ValidRegistrationNumber();    
    }
}