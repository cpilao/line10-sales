using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Line10.Sales.IntegrationTests.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Line10.Sales.IntegrationTests.Api;

public class OrderApiIntegrationTests: BaseApiIntegrationTest
{
    private readonly HttpClient _client;

    public OrderApiIntegrationTests(IntegrationApiTestFixture fixture) 
        : base(fixture)
    {
        _client = _fixture.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/orders";
        
        // create customer
        var customerId = await _client.CreateCustomer(
            "John",
            "Doe",
             "john.doe@example.com",
             "123-456-7890");
        // create product
        var productId = await _client.CreateProduct(
            "product1",
            "product 1 description",
            "SK00001");

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
        var orderInfo = await _client.CreateFullOrder();

        // Act
        var response = await _client.GetAsync($"{url}/{orderInfo.OrderId}");
        response.EnsureSuccessStatusCode();
        
        // Assert
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["customerId"]?.ToString().ShouldBe(orderInfo.CustomerId.ToString());
        content["orderId"]?.ToString().ShouldBe(orderInfo.OrderId.ToString());
        content["status"]?.ToString().ShouldBe("Pending");
    }
    
    [Fact]
    public async Task CancelOrder_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/orders";
        
        // create order
        var orderInfo = await _client.CreateFullOrder();

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
        var orderInfo = await _client.CreateFullOrder();

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
        var orderInfo = await _client.CreateFullOrder();
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
        var orderInfo = await _client.CreateFullOrder();
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
}