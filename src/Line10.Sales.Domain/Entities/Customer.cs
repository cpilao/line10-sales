using Line10.Sales.Core;

namespace Line10.Sales.Domain.Entities;

public class Customer
{
    private const string InvalidCustomerFirstName = nameof(InvalidCustomerFirstName);
    private const string InvalidCustomerLastName = nameof(InvalidCustomerLastName);
    private const string InvalidCustomerEmail = nameof(InvalidCustomerEmail);
    
    public Guid Id { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }

    private Customer() { }
    
    private Customer(
        string firstName,
        string lastName,
        string email,
        string? phone)
    {
        Id = Guid.NewGuid();
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Phone = phone;
    }
    
    public static Result<Customer?> Create(
        string firstName,
        string lastName,
        string email,
        string? phone)
    {
        var errors = new List<Error>();
        if (string.IsNullOrEmpty(firstName))
        {
            errors.Add(new Error(InvalidCustomerFirstName));
        }
        if (string.IsNullOrEmpty(lastName))
        {
            errors.Add(new Error(InvalidCustomerLastName));
        }
        if (string.IsNullOrEmpty(email))
        {
            errors.Add(new Error(InvalidCustomerEmail));
        }

        return errors.Count != 0
            ? Result.Create<Customer?>(errors.ToArray())
            : Result.Create<Customer?>(new Customer(firstName, lastName, email, phone));
    }
}