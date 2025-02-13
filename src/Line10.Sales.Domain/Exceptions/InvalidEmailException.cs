namespace Line10.Sales.Domain.Exceptions;

public sealed class InvalidEmailException(string message) : DomainException(InvalidEmail, message)
{
    private const string InvalidEmail = nameof(InvalidEmail);
}