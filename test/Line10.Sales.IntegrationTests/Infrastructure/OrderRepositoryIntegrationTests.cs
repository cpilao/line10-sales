using Line10.Sales.Domain.Entities;
using Shouldly;

namespace Line10.Sales.IntegrationTests.Infrastructure;

public class OrderRepositoryIntegrationTests : IClassFixture<IntegrationDataTestFixture>
{
    private readonly IntegrationDataTestFixture _fixture;

    public OrderRepositoryIntegrationTests(IntegrationDataTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddOrder_Should_AddOrderToDatabase()
    {
        // Arrange
        var product = Product
            .Create("product1", "product 1 description", "SK00001")
            .Content;
        product.ShouldNotBeNull();
        var customer = Customer
            .Create("John", "Doe", "john.doe@example.com", "123-456-7890")
            .Content;
        customer.ShouldNotBeNull();
        var order = Order
            .Create(customer.Id, product.Id)
            .Content;
        order.ShouldNotBeNull();
        
        // Act
        await _fixture.CustomerRepository.AddAsync(customer);
        await _fixture.ProductRepository.AddAsync(product);
        await _fixture.OrderRepository.AddAsync(order);

        // Assert
        var savedOrder = await _fixture.OrderRepository.GetByIdAsync(order.Id);
        savedOrder.ShouldNotBeNull();
        savedOrder.CustomerId.ShouldBe(order.CustomerId);
        savedOrder.OrderProducts.ShouldContain(o => o.ProductId.Equals(o.ProductId));
        savedOrder.Status.ShouldBe(OrderStatus.Pending);
    }
    
    [Fact]
    public async Task UpdateOrder_Should_UpdateOrderToDatabase()
    {
        // Arrange
        var product = Product
            .Create("product1", "product 1 description", "SK00001")
            .Content;
        product.ShouldNotBeNull();
        var customer = Customer
            .Create("John", "Doe", "john.doe@example.com", "123-456-7890")
            .Content;
        customer.ShouldNotBeNull();
        var order = Order
            .Create(customer.Id, product.Id)
            .Content;
        order.ShouldNotBeNull();
        
        await _fixture.CustomerRepository.AddAsync(customer);
        await _fixture.ProductRepository.AddAsync(product);
        await _fixture.OrderRepository.AddAsync(order);
        
        // Act
        order.Cancel();
        await _fixture.OrderRepository.UpdateAsync(order);

        // Assert
        var updatedOrder = await _fixture.OrderRepository.GetByIdAsync(order.Id);
        updatedOrder.ShouldNotBeNull();
        updatedOrder.CustomerId.ShouldBe(order.CustomerId);
        updatedOrder.OrderProducts.ShouldContain(o => o.ProductId.Equals(o.ProductId));
        updatedOrder.Status.ShouldBe(OrderStatus.Cancelled);
    }
    
    [Fact]
    public async Task RemoveOrder_Should_UpdateOrderToDatabase()
    {
        // Arrange
        var product = Product
            .Create("product1", "product 1 description", "SK00001")
            .Content;
        product.ShouldNotBeNull();
        var customer = Customer
            .Create("John", "Doe", "john.doe@example.com", "123-456-7890")
            .Content;
        customer.ShouldNotBeNull();
        var order = Order
            .Create(customer.Id, product.Id)
            .Content;
        order.ShouldNotBeNull();
        
        await _fixture.CustomerRepository.AddAsync(customer);
        await _fixture.ProductRepository.AddAsync(product);
        await _fixture.OrderRepository.AddAsync(order);
        
        // Act
        await _fixture.OrderRepository.DeleteAsync(order.Id);

        // Assert
        var deletedOrder = await _fixture.OrderRepository.GetByIdAsync(order.Id);
        deletedOrder.ShouldBeNull();
    }
}