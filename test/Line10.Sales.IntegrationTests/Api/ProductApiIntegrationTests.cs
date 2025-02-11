using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Line10.Sales.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Line10.Sales.IntegrationTests.Api;

public class ProductApiIntegrationTests: BaseApiIntegrationTest
{
    private readonly HttpClient _client;

    public ProductApiIntegrationTests(IntegrationApiTestFixture fixture) 
        : base(fixture)
    {
        _client = _fixture.CreateClient();
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/products";

        // Act
        var response = await _client.PostAsJsonAsync(url, new
        {
            name = "product1",
            description = "product 1 description",
            sku = "SK00001"
        });
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["productId"].ShouldNotBeNull();
        var productId = content["productId"]?.GetValue<Guid>();
        productId.ShouldNotBe(Guid.Empty);
    }
    
    [Fact]
    public async Task GetProduct_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/products";

        // Act
        var response = await _client.PostAsJsonAsync(url, new
        {
            name = "product1",
            description = "product 1 description",
            sku = "SK00001"
        });
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        var customerId = content["productId"]?.GetValue<Guid>();
        
        // Assert
        response = await _client.GetAsync($"{url}/{customerId}");
        response.EnsureSuccessStatusCode();
        
        content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["productId"]?.ToString().ShouldBe(customerId.ToString());
        content["name"]?.ToString().ShouldBe("product1");
        content["description"]?.ToString().ShouldBe("product 1 description");
        content["sku"]?.ToString().ShouldBe("SK00001");
    }
    
    [Fact]
    public async Task DeleteProduct_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/products";

        // Act
        var response = await _client.PostAsJsonAsync(url, new
        {
            name = "product1",
            description = "product 1 description",
            sku = "SK00001"
        });
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        var productId = content["productId"]?.GetValue<Guid>();
        
        // Assert
        response = await _client.DeleteAsync($"{url}/{productId}");
        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task UpdateProduct_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/products";

        // Act
        var response = await _client.PostAsJsonAsync(url, new
        {
            name = "product1",
            description = "product 1 description",
            sku = "SK00001"
        });
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        var productId = content["productId"]?.GetValue<Guid>();
        
        // Assert
        response = await _client.PutAsJsonAsync($"{url}/{productId}",new
        {
            name = "product2",
            description = "product 2 description",
            sku = "SK00002"
        });
        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        response = await _client.GetAsync($"{url}/{productId}");
        response.EnsureSuccessStatusCode();
        
        content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["productId"]?.ToString().ShouldBe(productId.ToString());
        content["name"]?.ToString().ShouldBe("product2");
        content["description"]?.ToString().ShouldBe("product 2 description");
        content["sku"]?.ToString().ShouldBe("SK00002");
    }
    
    [Theory]
    [InlineData(100,10, 10)]
    [InlineData(100,50, 2)]
    [InlineData(100,20, 5)]
    public async Task GetProducts_ShouldReturnPaginatedProducts(
        int productsCount,
        int pageSize,
        int expectedPagesNumber)
    {
        // Arrange
        var url = "/products";
        var pageNumber = 1;

        for (var i = 0; i < productsCount; i++)
        {
            var createResponse = await _client.PostAsJsonAsync(url, new
            {
                name = $"product_{i+1}",
                description = $"product description_{i+1}",
                sku = $"SK0000_{i+1}"
            });
            createResponse.EnsureSuccessStatusCode();
        }

        // Act
        while (productsCount != 0)
        {
            var response = await _client.GetAsync($"{url}?pageSize={pageSize}&pageNumber={pageNumber}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<JsonNode>();
            content.ShouldNotBeNull();
            content["products"].ShouldNotBeNull();

            var products = content["products"]!.AsArray();
            productsCount -= products.Count;
            if (productsCount != 0)
            {
                pageNumber++;
            }
        }

        // Assert
        pageNumber.ShouldBe(expectedPagesNumber);
    }
    
    [Theory]
    [InlineData("Name", "Desc", "product_c", "product_b", "product_a")]
    [InlineData("Name", "Asc", "product_a", "product_b", "product_c")]
    [InlineData("Description", "Desc", "product_c", "product_a", "product_b")]
    [InlineData("Description", "Asc", "product_b", "product_a", "product_c")]
    [InlineData("Sku", "Desc", "product_b", "product_c", "product_a")]
    [InlineData("Sku", "Asc", "product_a", "product_c", "product_b")]
    public async Task GetProducts_ShouldReturnOrderedProducts(
        string orderBy,
        string order,
        string expectedFirstProductName,
        string expectedSecondProductName,
        string expectedThirdProductName)
    {
        // Arrange
        var url = "/products";
        var testId = Guid.NewGuid().ToString();
        
        await _client.CreateProduct($"{testId}_product_a", "some description b", "SK0001");
        await _client.CreateProduct($"{testId}_product_b", "some description a", "SK0003");
        await _client.CreateProduct($"{testId}_product_c", "some description c", "SK0002");
        
        // Act
        var response = await _client.GetAsync($"{url}?orderBy={orderBy}&order={order}&name={testId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["products"].ShouldNotBeNull();

        var products = content["products"]!.AsArray();

        // Assert
        products.ShouldNotBeNull();
        products[0]?["name"]?.GetValue<string>().ShouldBe($"{testId}_{expectedFirstProductName}");
        products[1]?["name"]?.GetValue<string>().ShouldBe($"{testId}_{expectedSecondProductName}");
        products[2]?["name"]?.GetValue<string>().ShouldBe($"{testId}_{expectedThirdProductName}");
    }
}