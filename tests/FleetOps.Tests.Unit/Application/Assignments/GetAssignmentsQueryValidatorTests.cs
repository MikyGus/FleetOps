using FleetOps.Application.Assignments.GetAssignments;
using FleetOps.Application.Validations;
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
            .WithErrorCode(ValidationErrorCodes.Assignment.TimeRange.Invalid);
        result.ShouldHaveValidationErrorFor(x => x.ToUtc)
            .WithErrorCode(ValidationErrorCodes.Assignment.TimeRange.Invalid);
    }

    [Fact]
    public async Task Should_have_error_when_limit_is_zero()
    {
        var query = new GetAssignmentsQuery(
            null,
            null,
            null,
            null,
            0
        );

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Limit)
            .WithErrorCode(ValidationErrorCodes.Pagination.Limit.Invalid);
    }

    [Fact]
    public async Task Should_have_error_when_limit_exceeds_maximum_limit()
    {
        var query = new GetAssignmentsQuery(
            null,
            null,
            null,
            null,
            1000
        );

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Limit)
            .WithErrorCode(ValidationErrorCodes.Pagination.Limit.Invalid);
    }

    [Fact]
    public async Task Should_not_have_error_when_limit_at_maximum()
    {
        var query = new GetAssignmentsQuery(
            null,
            null,
            null,
            null,
            ValidationConstants.Pagination.MaxPageSize
        );

        var result = await _validator.TestValidateAsync(query);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_have_error_when_offset_is_negative()
    {
        var query = new GetAssignmentsQuery(
            null,
            null,
            null,
            null,
            10,
            -1
        );

        var result = await _validator.TestValidateAsync(query);

        result.ShouldHaveValidationErrorFor(x => x.Offset)
            .WithErrorCode(ValidationErrorCodes.Pagination.Offset.Invalid);
    }
}
