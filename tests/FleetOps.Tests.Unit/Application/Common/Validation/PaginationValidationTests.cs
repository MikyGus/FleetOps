using FluentValidation;
using FluentValidation.TestHelper;
using FleetOps.Application.Validations;
using System.Threading.Tasks;

namespace FleetOps.Tests.Unit.Application.Common.Validation;

public sealed record PaginationTestModel(int Limit, int Offset);
public sealed class PaginationTestValidator : AbstractValidator<PaginationTestModel>
{
    public PaginationTestValidator()
    {
        RuleFor(x => x.Limit).ValidLimit();
        RuleFor(x => x.Offset).ValidOffset();
    }
}

public sealed class PaginationValidationTests
{
    private readonly PaginationTestValidator _validator = new();

    [Fact]
    public async Task Should_have_error_when_limit_is_zero()
    {
        var model = new PaginationTestModel(0,1);

        var result = await _validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(x => x.Limit)
            .WithErrorCode(ValidationErrorCodes.Pagination.Limit.Invalid);
    }

    [Fact]
    public async Task Should_have_error_when_limit_exceeds_maximum_limit()
    {
        var model = new PaginationTestModel(1000,1);

        var result = await _validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(x => x.Limit)
            .WithErrorCode(ValidationErrorCodes.Pagination.Limit.Invalid);
    }

    [Fact]
    public async Task Should_not_have_error_when_limit_at_maximum()
    {
        var model = new PaginationTestModel(ValidationConstants.Pagination.MaxPageSize,1);

        var result = await _validator.TestValidateAsync(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_not_have_error_when_limit_at_minimum()
    {
        var model = new PaginationTestModel(ValidationConstants.Pagination.MinPageSize, 1);

        var result = await _validator.TestValidateAsync(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Should_have_error_when_offset_is_negative()
    {
        var model = new PaginationTestModel(10,-1);

        var result = await _validator.TestValidateAsync(model);

        result.ShouldHaveValidationErrorFor(x => x.Offset)
            .WithErrorCode(ValidationErrorCodes.Pagination.Offset.Invalid);
    }

    [Fact]
    public async Task Should_not_have_error_when_offset_is_zero()
    {
        var model = new PaginationTestModel(10, 0);

        var result = await _validator.TestValidateAsync(model);

        result.ShouldNotHaveAnyValidationErrors();
    }

}