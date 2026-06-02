using FleetOps.Application.Drivers.GetDrivers;
using FleetOps.Domain.Errors;
using FleetOps.Tests.Unit.Application.Common.Validation;
using FluentValidation.TestHelper;

namespace FleetOps.Tests.Unit.Application.Drivers;

public sealed class GetDriversQueryValidatorTests
{
    private readonly GetDriversQueryValidator _validator = new();

    [Fact]
    public async Task Should_have_error_when_length_of_name_exceeds_maximum_limit()
    {
        var query = new GetDriversQuery(
            new string('X', 201),
            null
        );

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Name)
            .WithErrorCode(ErrorCodes.Driver.Name.MaxLengthExceeded);
    }

    [Fact]
    public async Task Should_not_have_error_when_name_is_shorter_than_maximum_length()
    {
        var query = new GetDriversQuery("XX", null);

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_not_have_error_for_valid_pagination()
    {
        var query = new GetDriversQuery(null, null, 50, 0);

        await ValidationAssert.HasNoError(_validator, query, x => x.Limit);
        await ValidationAssert.HasNoError(_validator, query, x => x.Offset);
    }

    [Fact]
    public async Task Should_have_error_when_limit_is_invalid()
    {
        var query = new GetDriversQuery(null, null, 1000, 0);

        await ValidationAssert.HasError(_validator, query, x => x.Limit, ErrorCodes.Pagination.Limit.Invalid);
    }

    [Fact]
    public async Task Should_have_error_when_offset_is_invalid()
    {
        var query = new GetDriversQuery(null, null, 50, -1);

        await ValidationAssert.HasError(_validator, query, x => x.Offset, ErrorCodes.Pagination.Offset.Invalid);
    }

}