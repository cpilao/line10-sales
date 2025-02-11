using Line10.Sales.Domain.Entities;
using Shouldly;

namespace Line10.Sales.UnitTests.Domain;

public class OrderTests
{
    [Fact]
    public void Create_ShouldReturnOrder_WhenValidParameters()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Act
        var result = Order.Create(customerId, productId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Content.ShouldNotBeNull();
        result.Content.Id.ShouldNotBe(Guid.Empty);
        result.Content.CustomerId.ShouldBe(customerId);
        result.Content.OrderProducts.ShouldContain(product => product.ProductId.Equals(productId));
        result.Content.Status.ShouldBe(OrderStatus.Pending);
        result.Content.CreateDate.ShouldBeInRange(DateTime.Now.AddSeconds(-1), DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCustomerIdIsEmpty()
    {
        // Arrange
        var customerId = Guid.Empty;
        var productId = Guid.NewGuid();

        // Act
        var result = Order.Create(customerId, productId);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "InvalidOrderCustomerId");
    }

    [Fact]
    public void Create_ShouldReturnError_WhenProductIdIsEmpty()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.Empty;

        // Act
        var result = Order.Create(customerId, productId);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "InvalidOrderProductId");
    }
}