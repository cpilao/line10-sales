using Line10.Sales.Domain.Entities;
using Shouldly;

namespace Line10.Sales.UnitTests.Domain;

public class ProductTests
{
    [Fact]
    public void Create_ShouldReturnProduct_WhenValidParameters()
    {
        // Arrange
        var name = "Product Name";
        var description = "Product Description";
        var sku = "SKU123";

        // Act
        var result = Product.Create(name, description, sku);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Content.ShouldNotBeNull();
        result.Content.Id.ShouldNotBe(Guid.Empty);
        result.Content.Name.ShouldBe(name);
        result.Content.Description.ShouldBe(description);
        result.Content.Sku.ShouldBe(sku);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNameIsEmpty()
    {
        // Arrange
        var name = string.Empty;
        var description = "Product Description";
        var sku = "SKU123";

        // Act
        var result = Product.Create(name, description, sku);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "InvalidProductName");
    }

    [Fact]
    public void Create_ShouldReturnError_WhenDescriptionIsEmpty()
    {
        // Arrange
        var name = "Product Name";
        var description = string.Empty;
        var sku = "SKU123";

        // Act
        var result = Product.Create(name, description, sku);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "InvalidProductDescription");
    }

    [Fact]
    public void Create_ShouldReturnError_WhenSkuIsEmpty()
    {
        // Arrange
        var name = "Product Name";
        var description = "Product Description";
        var sku = string.Empty;

        // Act
        var result = Product.Create(name, description, sku);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.Code == "InvalidProductSku");
    }
}