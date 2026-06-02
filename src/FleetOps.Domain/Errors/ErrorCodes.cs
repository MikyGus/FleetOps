namespace FleetOps.Domain.Errors;

public static class ErrorCodes
{
    public static class Assignment
    {
        public static class DriverId
        {
            public const string Required = "Assignment.DriverId.Required";
            public const string NotFound = "Assignment.DriverId.NotFound";
            public const string Overlap = "Assignment.DriverId.Overlap";
        }

        public static class VehicleId
        {
            public const string Required = "Assignment.VehicleId.Required";
            public const string NotFound = "Assignment.VehicleId.NotFound";
            public const string Overlap = "Assignment.VehicleId.Overlap";
        }

        public static class TimeRange
        {
            public const string Invalid = "Assignment.TimeRange.Invalid";
        }
    }

    public static class Driver
    {
        public static class Name
        {
            public const string Required = "Driver.Name.Required";
            public const string MaxLengthExceeded = "Driver.Name.MaxLengthExceeded";
        }
    }

    public static class Pagination
    {
        public static class Limit
        {
            public const string Invalid = "Pagination.Limit.Invalid";
        }

        public static class Offset
        {
            public const string Invalid = "Pagination.Offset.Invalid";
        }
    }

    public static class Vehicle
    {
        public static class RegistrationNumber
        {
            public const string Required = "Vehicle.RegistrationNumber.Required";
            public const string MaxLengthExceeded = "Vehicle.RegistrationNumber.MaxLengthExceeded";
        }
    }
}
