using System.Text.RegularExpressions;
using Line10.Sales.Domain.Exceptions;

namespace Line10.Sales.Domain.ValueObjects;

public partial record Email
{
    public static readonly Email Empty = new();
    private const string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    
    public string Address { get; }

    private Email()
    {
        Address = string.Empty;
    }

    public Email(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new InvalidEmailException("Email address cannot be empty.");
        }

        if (!IsValidEmail(address))
        {
            throw new InvalidEmailException("Invalid email address format.");
        }

        Address = address;
    }

    private static bool IsValidEmail(string email)
    {
        return MyRegex().IsMatch(email);
    }

    public override string ToString() => Address;
    
    public static implicit operator Email(string address) => new Email(address);
    
    [GeneratedRegex(EmailPattern)]
    private static partial Regex MyRegex();
}