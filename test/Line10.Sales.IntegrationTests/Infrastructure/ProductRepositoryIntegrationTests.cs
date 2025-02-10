using Line10.Sales.Domain.Entities;
using Shouldly;

namespace Line10.Sales.IntegrationTests.Infrastructure;

public class ProductRepositoryIntegrationTests : IClassFixture<IntegrationDataTestFixture>
{
    private readonly IntegrationDataTestFixture _fixture;

    public ProductRepositoryIntegrationTests(IntegrationDataTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AddProduct_ShouldAddProductToDatabase()
    {
        // Arrange
        var product = Product
            .Create("product1", "product 1 description", "SK00001")
            .Content;
        product.ShouldNotBeNull();
        
        // Act
        await _fixture.ProductRepository.AddAsync(product);

        // Assert
        var savedProduct = await _fixture.ProductRepository.GetByIdAsync(product.Id);
        savedProduct.ShouldNotBeNull();
        savedProduct.Name.ShouldBe("product1");
        savedProduct.Description.ShouldBe("product 1 description");
        savedProduct.Sku.ShouldBe("SK00001");
    }
    
    [Fact]
    public async Task UpdateProduct__ShouldUpdateProduct_ToDatabase()
    {
        // Arrange
        var product = Product
            .Create("product1", "product 1 description", "SK00001")
            .Content;
        product.ShouldNotBeNull();
        await _fixture.ProductRepository.AddAsync(product);

        product.UpdateName("product name updated");
        product.UpdateDescription("product description updated");
        product.UpdateSku("SK0000U");
        
        // Act
        await _fixture.ProductRepository.UpdateAsync(product);

        // Assert
        var savedProduct = await _fixture.ProductRepository.GetByIdAsync(product.Id);
        savedProduct.ShouldNotBeNull();
        savedProduct.Name.ShouldBe("product name updated");
        savedProduct.Description.ShouldBe("product description updated");
        savedProduct.Sku.ShouldBe("SK0000U");
    }

    [Fact]
    public async Task DeleteProduct_ShouldRemoveProductFromDatabase()
    {
        // Arrange
        var product = Product
            .Create("product1", "product 1 description", "SK00001")
            .Content;
        product.ShouldNotBeNull();
        await _fixture.ProductRepository.AddAsync(product);
        
        // Act
        await _fixture.ProductRepository.DeleteAsync(product.Id);

        // Assert
        var deletedProduct = await _fixture.ProductRepository.GetByIdAsync(product.Id);
        deletedProduct.ShouldBeNull();
    }
}