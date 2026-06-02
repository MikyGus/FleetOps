using FleetOps.Application.Vehicles.GetVehicles;
using FleetOps.Domain.Errors;
using FleetOps.Tests.Unit.Application.Common.Validation;
using FluentValidation.TestHelper;

namespace FleetOps.Tests.Unit.Application.Vehicles;

public sealed class GetVehiclesQueryValidatorTests
{
    private readonly GetVehiclesQueryValidator _validator = new();

    [Fact]
    public async Task Should_have_error_when_length_of_registrationnumber_exceeds_maximum()
    {
        var query = new GetVehiclesQuery(new string('X', 21), null);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.RegistrationNumber)
            .WithErrorCode(ErrorCodes.Vehicle.RegistrationNumber.MaxLengthExceeded);
    }

    [Fact]
    public async Task Should_not_have_error_when_query_is_valid()
    {
        var query = new GetVehiclesQuery("XX", null);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_not_have_error_for_valid_pagination()
    {
        var query = new GetVehiclesQuery(null, null, 50, 0);

        await ValidationAssert.HasNoError(_validator, query, x => x.Limit);
        await ValidationAssert.HasNoError(_validator, query, x => x.Offset);
    }

    [Fact]
    public async Task Should_have_error_when_limit_is_invalid()
    {
        var query = new GetVehiclesQuery(null, null, 1000, 0);

        await ValidationAssert.HasError(_validator, query, x => x.Limit, ErrorCodes.Pagination.Limit.Invalid);
    }

    [Fact]
    public async Task Should_have_error_when_offset_is_invalid()
    {
        var query = new GetVehiclesQuery(null, null, 50, -1);

        await ValidationAssert.HasError(_validator, query, x => x.Offset, ErrorCodes.Pagination.Offset.Invalid);
    }
}
