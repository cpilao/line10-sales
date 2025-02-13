namespace Line10.Sales.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public string ErrorCode { get; }

    protected DomainException(string errorCode, string? message = null)
        : base(message)
    {
        ErrorCode = errorCode;
    }
}