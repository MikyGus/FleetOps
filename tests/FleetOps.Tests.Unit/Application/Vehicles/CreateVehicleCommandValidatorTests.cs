using FleetOps.Application.Validations;
using FleetOps.Application.Vehicles.CreateVehicle;
using FleetOps.Domain.Errors;
using FluentValidation.TestHelper;

namespace FleetOps.Tests.Unit.Application.Vehicles;

public sealed class CreateVehicleCommandValidatorTests
{
    private readonly CreateVehicleCommandValidator _validator = new();

    [Fact]
    public async Task Should_have_error_when_length_of_registrationnumber_exceeds_maximum()
    {
        var command = new CreateVehicleCommand(new string('X', 21));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.RegistrationNumber)
            .WithErrorCode(ErrorCodes.Vehicle.RegistrationNumber.MaxLengthExceeded);
    }

    [Fact]
    public async Task Should_have_error_when_registrationnumber_is_empty()
    {
        var command = new CreateVehicleCommand("");

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.RegistrationNumber)
            .WithErrorCode(ErrorCodes.Vehicle.RegistrationNumber.Required);
    }

    [Fact]
    public async Task Should_not_have_errors_when_length_of_registrationnumber_is_on_maximum()
    {
        var command = new CreateVehicleCommand(new string('X', ValidationConstants.RegistrationNumber.MaxLength));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_not_have_errors_when_command_is_valid()
    {
        var command = new CreateVehicleCommand("XX");

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
