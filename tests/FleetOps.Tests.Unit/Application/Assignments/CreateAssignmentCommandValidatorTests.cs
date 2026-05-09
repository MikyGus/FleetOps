using FleetOps.Application.Assignments.CreateAssignment;
using FleetOps.Application.Validations;
using FleetOps.Domain.Drivers;
using FleetOps.Domain.Errors;
using FleetOps.Domain.Vehicles;
using FluentValidation.TestHelper;
using Moq;

namespace FleetOps.Tests.Unit.Application.Assignments;

public sealed class CreateAssignmentCommandValidatorTests
{
    private readonly Mock<IEntityExistenceChecker<Driver, Guid>> _driverChecker = new();
    private readonly Mock<IEntityExistenceChecker<Vehicle, Guid>> _vehicleChecker = new();
    private readonly CreateAssignmentCommandValidator _validator;

    public CreateAssignmentCommandValidatorTests()
    {
        _validator = new(_driverChecker.Object, _vehicleChecker.Object);
    }

    [Fact]
    public async Task Should_have_error_when_driver_id_is_empty()
    {
        _driverChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _vehicleChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var startUtc = new DateTimeOffset(2026, 4, 5, 10, 0, 0, TimeSpan.Zero);
        var endUtc = new DateTimeOffset(2026, 4, 5, 11, 0, 0, TimeSpan.Zero);

        var command = new CreateAssignmentCommand(
            Guid.Empty,
            Guid.NewGuid(),
            startUtc,
            endUtc);
        
        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.DriverId)
            .WithErrorCode(ErrorCodes.Assignment.DriverId.Required);
    }       

    [Fact]
    public async Task Should_have_error_when_vehicle_id_is_empty()
    {
        _driverChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _vehicleChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var startUtc = new DateTimeOffset(2026, 4, 5, 10, 0, 0, TimeSpan.Zero);
        var endUtc = new DateTimeOffset(2026, 4, 5, 11, 0, 0, TimeSpan.Zero);

        var command = new CreateAssignmentCommand(
            Guid.NewGuid(),
            Guid.Empty,
            startUtc,
            endUtc);

        
        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.VehicleId)
            .WithErrorCode(ErrorCodes.Assignment.VehicleId.Required);
    }

    [Fact]
    public async Task Should_have_error_if_driver_does_not_exist()
    {
        _driverChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _vehicleChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var startUtc = new DateTimeOffset(2026, 4, 5, 10, 0, 0, TimeSpan.Zero);
        var endUtc = new DateTimeOffset(2026, 4, 5, 11, 0, 0, TimeSpan.Zero);

        var command = new CreateAssignmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            startUtc,
            endUtc);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.DriverId)
            .WithErrorCode(ErrorCodes.Assignment.DriverId.NotFound);
    }             

    [Fact]
    public async Task Should_have_error_if_vehicle_does_not_exist()
    {
        _driverChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _vehicleChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var startUtc = new DateTimeOffset(2026, 4, 5, 10, 0, 0, TimeSpan.Zero);
        var endUtc = new DateTimeOffset(2026, 4, 5, 11, 0, 0, TimeSpan.Zero);

        var command = new CreateAssignmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            startUtc,
            endUtc);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.VehicleId)
            .WithErrorCode(ErrorCodes.Assignment.VehicleId.NotFound);
    }

    [Fact]
    public async Task Should_have_error_when_endUtc_is_before_startUtc()
    {
        _driverChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _vehicleChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var startUtc = new DateTimeOffset(2026, 4, 5, 12, 0, 0, TimeSpan.Zero);
        var endUtc = new DateTimeOffset(2026, 4, 5, 11, 0, 0, TimeSpan.Zero);

        var command = new CreateAssignmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            startUtc,
            endUtc);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(x => x.StartUtc)
            .WithErrorCode(ErrorCodes.Assignment.TimeRange.Invalid);
        result.ShouldHaveValidationErrorFor(x => x.EndUtc)
            .WithErrorCode(ErrorCodes.Assignment.TimeRange.Invalid);
    }

    [Fact]
    public async Task Should_not_have_error_when_command_is_valid()
    {
        _driverChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _vehicleChecker
            .Setup(x => x.ExistsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var startUtc = new DateTimeOffset(2026, 4, 5, 10, 0, 0, TimeSpan.Zero);
        var endUtc = new DateTimeOffset(2026, 4, 5, 11, 0, 0, TimeSpan.Zero);

        var command = new CreateAssignmentCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            startUtc,
            endUtc);

        var result = await _validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
}