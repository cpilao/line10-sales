using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Line10.Sales.IntegrationTests.Api;

public class ProductApiIntegrationTests: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ProductApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
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
}