namespace FleetOps.Domain.Exceptions;

public sealed class DomainValidationException : Exception
{
    public string PropertyName { get; }
    public string ErrorCode { get; }

    public DomainValidationException(
        string propertyName,
        string errorCode,
        string message)
        : base(message)
    {
        PropertyName = propertyName;
        ErrorCode = errorCode;
    }
}