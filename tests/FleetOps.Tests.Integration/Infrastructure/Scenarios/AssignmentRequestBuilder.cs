using FleetOps.Tests.Integration.Contracts.Assignments;
using FleetOps.Tests.Integration.Infrastructure.Fixtures;

namespace FleetOps.Tests.Integration.Infrastructure.Scenarios;

public sealed class AssignmentRequestBuilder
{
    private Guid _driverId;
    private Guid _vehicleId;
    private DateTimeOffset _startUtc = TimeTestFixtures.Period1.Start;
    private DateTimeOffset _endUtc = TimeTestFixtures.Period1.End_Valid;

    private AssignmentRequestBuilder(Guid driverId, Guid vehicleId)
    {
        _driverId = driverId;
        _vehicleId = vehicleId;
    }

    public static AssignmentRequestBuilder For(Guid driverId, Guid vehicleId)
        => new(driverId, vehicleId);

    public static AssignmentRequestBuilder WithMissingDriver(Guid vehicleId)
        => new(Guid.NewGuid(), vehicleId);

    public static AssignmentRequestBuilder WithMissingVehicle(Guid driverId)
        => new(driverId, Guid.NewGuid());

    public AssignmentRequestBuilder WithEndBeforeStart()
    {
        _startUtc = TimeTestFixtures.Period1.Start;
        _endUtc = TimeTestFixtures.Period1.End_Invalid_BeforeStart;
        return this;
    }

    public AssignmentRequestBuilder InPeriod1()
    {
        _startUtc = TimeTestFixtures.Period1.Start;
        _endUtc = TimeTestFixtures.Period1.End_Valid;
        return this;
    }

    public AssignmentRequestBuilder OverlappingPeriod1()
    {
        _startUtc = TimeTestFixtures.Period2.Start_Invalid_ConflictWithPeriod1;
        _endUtc = TimeTestFixtures.Period2.End_Valid;
        return this;
    }

    public AssignmentRequestBuilder BackToBackAfterPeriod1()
    {
        _startUtc = TimeTestFixtures.Period2.Start_Valid_Back2BackWithPeriod1End;
        _endUtc = TimeTestFixtures.Period2.End_Valid;
        return this;
    }

    public AssignmentRequestBuilder AfterPeriod1()
    {
        _startUtc = TimeTestFixtures.Period2.Start_Valid_AfterValidEnd;
        _endUtc = TimeTestFixtures.Period2.End_Valid;
        return this;
    }

    public CreateAssignmentRequest Build()
        => new(_driverId, _vehicleId, _startUtc, _endUtc);
}
