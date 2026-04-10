using FleetOps.Application.Drivers.GetDrivers;
using FleetOps.Application.Validations;
using FluentValidation.TestHelper;

namespace FleetOps.Tests.Unit.Application.Drivers;

public sealed class GetDriverQueryValidatorTests
{
    private readonly GetDriversQueryValidator _validator = new();

    [Fact]
    public async Task Should_have_error_when_length_of_name_exceeds_maximum_limit()
    {
        var query = new GetDriversQuery(
            new string('X',201),
            null
        );

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ValidationErrorCodes.Driver.Name.MaxLengthExceeded);
    }

    [Fact]
    public async Task Should_not_have_error_when_name_is_shorter_than_maximum_length()
    {
        var query = new GetDriversQuery("XX",null);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }
}