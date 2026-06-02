using FleetOps.Application.Drivers.CreateDriver;
using FleetOps.Application.Validations;
using FleetOps.Domain.Errors;
using FluentValidation.TestHelper;

namespace FleetOps.Tests.Unit.Application.Drivers;

public sealed class CreateDriverCommandValidationTests
{
    private readonly CreateDriverCommandValidator _validator = new();

    [Fact]
    public async Task Should_have_error_when_driver_name_is_empty()
    {
        var command = new CreateDriverCommand(string.Empty);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ErrorCodes.Driver.Name.Required);
    }

    [Fact]
    public async Task Should_have_error_when_driver_name_exceeds_maximum_allowed_length()
    {
        var command = new CreateDriverCommand(new string('X', 201));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ErrorCodes.Driver.Name.MaxLengthExceeded);
    }

    [Fact]
    public async Task Should_not_have_error_when_length_of_name_at_maximum()
    {
        var command = new CreateDriverCommand(new string(
            'X',
            ValidationConstants.Names.MaxLength));

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
