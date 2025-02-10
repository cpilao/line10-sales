using Line10.Sales.Domain.Entities;
using Shouldly;

namespace Line10.Sales.IntegrationTests.Infrastructure;

public class CustomerRepositoryIntegrationTests : IClassFixture<IntegrationDataTestFixture>
{
    private readonly IntegrationDataTestFixture _fixture;

    public CustomerRepositoryIntegrationTests(IntegrationDataTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddCustomer_Should_AddCustomerToDatabase()
    {
        // Arrange
        var customer = Customer
            .Create("John", "Doe", "john.doe@example.com", "123-456-7890")
            .Content;
        customer.ShouldNotBeNull();
        
        // Act
        await _fixture.CustomerRepository.AddAsync(customer);

        // Assert
        var savedCustomer = await _fixture.CustomerRepository.GetByIdAsync(customer.Id);
        savedCustomer.ShouldNotBeNull();
        savedCustomer.FirstName.ShouldBe("John");
        savedCustomer.LastName.ShouldBe("Doe");
        savedCustomer.Email.ShouldBe("john.doe@example.com");
        savedCustomer.Phone.ShouldBe("123-456-7890");
    }
    
    [Fact]
    public async Task UpdateCustomer_Should_UpdateCustomerToDatabase()
    {
        // Arrange
        var customer = Customer
            .Create("John", "Doe", "john.doe@example.com", "123-456-7890")
            .Content;
        customer.ShouldNotBeNull();
        await _fixture.CustomerRepository.AddAsync(customer);

        customer.UpdateName("Charles", "Smith");
        customer.UpdateEmail("charles.smith@example.com");
        customer.UpdatePhone("123-456-7891");
        
        // Act
        await _fixture.CustomerRepository.UpdateAsync(customer);

        // Assert
        var savedCustomer = await _fixture.CustomerRepository.GetByIdAsync(customer.Id);
        savedCustomer.ShouldNotBeNull();
        savedCustomer.FirstName.ShouldBe("Charles");
        savedCustomer.LastName.ShouldBe("Smith");
        savedCustomer.Email.ShouldBe("charles.smith@example.com");
        savedCustomer.Phone.ShouldBe("123-456-7891");
    }

    [Fact]
    public async Task DeleteCustomer_Should_RemoveCustomerFromDatabase()
    {
        // Arrange
        var customer = Customer
            .Create("Jane", "Doe", "jane.doe@example.com", "098-765-4321")
            .Content;
        customer.ShouldNotBeNull();

        await _fixture.CustomerRepository.AddAsync(customer);

        // Act
        await _fixture.CustomerRepository.DeleteAsync(customer.Id);

        // Assert
        var deletedCustomer = await _fixture.CustomerRepository.GetByIdAsync(customer.Id);
        deletedCustomer.ShouldBeNull();
    }
}