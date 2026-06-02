using FleetOps.Application.Assignments.GetAssignments;
using FleetOps.Domain.Errors;
using FleetOps.Tests.Unit.Application.Common.Validation;
using FluentValidation.TestHelper;

namespace FleetOps.Tests.Unit.Application.Assignments;

public sealed class GetAssignmentsQueryValidatorTests()
{
    private readonly GetAssignmentsQueryValidator _validator = new();

    [Fact]
    public async Task Should_have_error_when_endUtc_is_before_startUtc()
    {
        var startUtc = new DateTimeOffset(2026, 4, 5, 12, 0, 0, TimeSpan.Zero);
        var endUtc = new DateTimeOffset(2026, 4, 5, 11, 0, 0, TimeSpan.Zero);

        var query = new GetAssignmentsQuery(
            null,
            null,
            startUtc,
            endUtc
        );

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.FromUtc)
            .WithErrorCode(ErrorCodes.Assignment.TimeRange.Invalid);
        result.ShouldHaveValidationErrorFor(x => x.ToUtc)
            .WithErrorCode(ErrorCodes.Assignment.TimeRange.Invalid);
    }

    [Fact]
    public async Task Should_not_have_error_for_valid_pagination()
    {
        var query = new GetAssignmentsQuery(null, null, null, null, 50, 0);

        await ValidationAssert.HasNoError(_validator, query, x => x.Limit);
        await ValidationAssert.HasNoError(_validator, query, x => x.Offset);
    }

    [Fact]
    public async Task Should_have_error_when_limit_is_invalid()
    {
        var query = new GetAssignmentsQuery(null, null, null, null, 1000, 0);

        await ValidationAssert.HasError(_validator, query, x => x.Limit, ErrorCodes.Pagination.Limit.Invalid);
    }

    [Fact]
    public async Task Should_have_error_when_offset_is_invalid()
    {
        var query = new GetAssignmentsQuery(null, null, null, null, 50, -1);

        await ValidationAssert.HasError(_validator, query, x => x.Offset, ErrorCodes.Pagination.Offset.Invalid);
    }
}
