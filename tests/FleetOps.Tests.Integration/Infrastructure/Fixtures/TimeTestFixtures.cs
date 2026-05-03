namespace FleetOps.Tests.Integration.Infrastructure.Fixtures;

public static class TimeTestFixtures
{
    public static class Period1
    {
        public static DateTimeOffset Start => new(2026, 4, 5, 10, 0, 0, TimeSpan.Zero);
        public static DateTimeOffset End_Valid => Start.AddHours(1);
        public static DateTimeOffset End_Invalid_BeforeStart => Start.AddHours(-1);
        public static DateTimeOffset End_Invalid_SameAsStart => Start;
    }

    public static class Period2
    {
        public static DateTimeOffset Start_Valid_AfterValidEnd => Period1.End_Valid.AddHours(1);
        public static DateTimeOffset Start_Valid_Back2BackWithPeriod1End => Period1.End_Valid;
        public static DateTimeOffset Start_Invalid_ConflictWithPeriod1 => Period1.Start.AddMinutes(30);
        public static DateTimeOffset End_Valid => Start_Valid_AfterValidEnd.AddHours(1);
    }
}