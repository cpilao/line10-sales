using Line10.Sales.Domain.Entities;
using Shouldly;

namespace Line10.Sales.UnitTests.Domain;


public class CustomerTests
{
    [Fact]
    public void Create_ShouldReturnCustomer_WhenValidParameters()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phone = "123-456-7890";

        // Act
        var result = Customer.Create(firstName, lastName, email, phone);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Content.ShouldNotBeNull();
        result.Content.Id.ShouldNotBe(Guid.Empty);
        result.Content.FirstName.ShouldBe(firstName);
        result.Content.LastName.ShouldBe(lastName);
        result.Content.Email.ShouldBe(email);
        result.Content.Phone.ShouldBe(phone);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenFirstNameIsEmpty()
    {
        // Arrange
        var firstName = string.Empty;
        var lastName = "Doe";
        var email = "john.doe@example.com";
        var phone = "123-456-7890";

        // Act
        var result = Customer.Create(firstName, lastName, email, phone);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "InvalidCustomerFirstName");
    }

    [Fact]
    public void Create_ShouldReturnError_WhenLastNameIsEmpty()
    {
        // Arrange
        var firstName = "John";
        var lastName = string.Empty;
        var email = "john.doe@example.com";
        var phone = "123-456-7890";

        // Act
        var result = Customer.Create(firstName, lastName, email, phone);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Message == "InvalidCustomerLastName");
    }

    [Fact]
    public void Create_ShouldReturnError_WhenEmailIsEmpty()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = string.Empty;
        var phone = "123-456-7890";

        // Act
        var result = Customer.Create(firstName, lastName, email, phone);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Message == "InvalidCustomerEmail");
    }
}