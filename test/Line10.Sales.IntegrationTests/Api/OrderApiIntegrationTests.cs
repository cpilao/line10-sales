using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Line10.Sales.IntegrationTests.Api;

public class OrderApiIntegrationTests: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OrderApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/orders";
        
        // create customer
        var customerId = await CreateCustomer();
        // create product
        var productId = await CreateProduct();

        // Act
        var response = await _client.PostAsJsonAsync(url, new
        {
            customerId,
            productId
        });
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["orderId"].ShouldNotBeNull();
        var orderId = content["orderId"]?.GetValue<Guid>();
        orderId.ShouldNotBe(Guid.Empty);
    }
    
    [Fact]
    public async Task GetOrder_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/orders";
        
        // create order
        var orderInfo = await CreateOrder();

        // Act
        var response = await _client.GetAsync($"{url}/{orderInfo.OrderId}");
        response.EnsureSuccessStatusCode();
        
        // Assert
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["customerId"]?.ToString().ShouldBe(orderInfo.CustomerId.ToString());
        content["productId"]?.ToString().ShouldBe(orderInfo.ProductId.ToString());
        content["orderId"]?.ToString().ShouldBe(orderInfo.OrderId.ToString());
        content["status"]?.ToString().ShouldBe("Pending");
    }
    
    [Fact]
    public async Task CancelOrder_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/orders";
        
        // create order
        var orderInfo = await CreateOrder();

        // Act
        var response = await _client.PostAsync($"{url}/{orderInfo.OrderId}/cancel", new StringContent(string.Empty));
        response.EnsureSuccessStatusCode();
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        response = await _client.GetAsync($"{url}/{orderInfo.OrderId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["status"]?.ToString().ShouldBe("Cancelled");
    }
    
    [Fact]
    public async Task ProcessOrder_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/orders";
        
        // create order
        var orderInfo = await CreateOrder();

        // Act
        var response = await _client.PostAsync($"{url}/{orderInfo.OrderId}/process", new StringContent(string.Empty));
        response.EnsureSuccessStatusCode();
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        response = await _client.GetAsync($"{url}/{orderInfo.OrderId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["status"]?.ToString().ShouldBe("Processing");
    }
    
    [Fact]
    public async Task ShipOrder_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/orders";
        
        // create order and process
        var orderInfo = await CreateOrder();
        var response = await _client.PostAsync($"{url}/{orderInfo.OrderId}/process", new StringContent(string.Empty));
        response.EnsureSuccessStatusCode();

        // Act
        response = await _client.PostAsync($"{url}/{orderInfo.OrderId}/ship", new StringContent(string.Empty));
        response.EnsureSuccessStatusCode();
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        response = await _client.GetAsync($"{url}/{orderInfo.OrderId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["status"]?.ToString().ShouldBe("Shipped");
    }
    
    [Fact]
    public async Task DeliveryOrder_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/orders";
        
        // create order and process
        var orderInfo = await CreateOrder();
        var response = await _client.PostAsync($"{url}/{orderInfo.OrderId}/process", new StringContent(string.Empty));
        response.EnsureSuccessStatusCode();
        response = await _client.PostAsync($"{url}/{orderInfo.OrderId}/ship", new StringContent(string.Empty));
        response.EnsureSuccessStatusCode();

        // Act
        response = await _client.PostAsync($"{url}/{orderInfo.OrderId}/delivery", new StringContent(string.Empty));
        response.EnsureSuccessStatusCode();
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        response = await _client.GetAsync($"{url}/{orderInfo.OrderId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["status"]?.ToString().ShouldBe("Delivered");
    }
    
    private async Task<Guid> CreateCustomer()
    {
        var url = "/customers";
        var response = await _client.PostAsJsonAsync(url, new
        {
            firstName = "John",
            lastName = "Doe",
            email = "john.doe@example.com",
            phone = "123-456-7890"
        });
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        var customerId = content["customerId"]?.GetValue<Guid>();
        customerId.ShouldNotBeNull();
        return customerId.Value;
    }
    
    private async Task<Guid> CreateProduct()
    {
        var url = "/products";
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
        productId.ShouldNotBeNull();
        return productId.Value;
    }
    
    private async Task<(Guid OrderId, Guid CustomerId, Guid ProductId)> CreateOrder()
    {
        var url = "/orders";
        
        // create customer
        var customerId = await CreateCustomer();
        // create product
        var productId = await CreateProduct();
        // create order
        var response = await _client.PostAsJsonAsync(url, new
        {
            customerId,
            productId
        });
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["orderId"].ShouldNotBeNull();
        var orderId = content["orderId"]?.GetValue<Guid>();
        orderId.ShouldNotBe(Guid.Empty);
        orderId.ShouldNotBeNull();
        return (orderId.Value, customerId, productId);
    }
}