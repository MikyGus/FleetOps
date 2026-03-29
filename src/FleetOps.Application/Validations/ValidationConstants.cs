namespace FleetOps.Application.Validations;

public static class ValidationConstants
{
    public static class Names
    {
        public const int MaxLength = 200;
    }

    public static class Pagination
    {
        public const int MinPageSize = 1;
        public const int MaxPageSize = 500;
    }

    public static class RegistrationNumber
    {
        public const int MaxLength = 20;
        public const string MessageTemplate = "{PropertyName} is {TotalLength}, but must be at most {MaxLength} characters long.";
    }
}