using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Line10.Sales.Core.Security;
using Line10.Sales.IntegrationTests.Extensions;
using Shouldly;

namespace Line10.Sales.IntegrationTests.Api;

public class OrderApiIntegrationTests: BaseApiIntegrationTest
{
    private readonly HttpClient _client;

    public OrderApiIntegrationTests(IntegrationApiTestFixture fixture) 
        : base(fixture)
    {
        _client = _fixture.CreateClient();
        var token = JwtUtils.GetToken([
            "orders.read",
            "orders.write",
            "customers.read",
            "customers.write",
            "products.read",
            "products.write"
        ]);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
    
    [Fact]
    public async Task AddOrderProduct_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/orders";
        
        // create order
        var orderInfo = await _client.CreateFullOrder();
        var productIdA = await _client.CreateProduct("another product a", "product a description", "SK0001A");
        var productIdB = await _client.CreateProduct("another product b", "product b description", "SK0001B");

        // Act
        var response = await _client.PostAsJsonAsync($"{url}/{orderInfo.OrderId}/products", new
        {
            quantity = 1,
            productId = productIdA
        });
        response.EnsureSuccessStatusCode();
        response = await _client.PostAsJsonAsync($"{url}/{orderInfo.OrderId}/products", new
        {
            quantity = 1,
            productId = productIdB
        });
        response.EnsureSuccessStatusCode();
        response = await _client.PostAsJsonAsync($"{url}/{orderInfo.OrderId}/products", new
        {
            quantity = 1,
            productId = productIdB
        });
        response.EnsureSuccessStatusCode();
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        response = await _client.GetAsync($"{url}/{orderInfo.OrderId}/products");
        response.EnsureSuccessStatusCode();
        var orderProducts = await response.Content.ReadFromJsonAsync<JsonArray>();
        orderProducts.ShouldNotBeNull();
        orderProducts.ShouldNotBeNull();
        orderProducts.Count.ShouldBe(3);
        orderProducts.Sum(node => node?["quantity"]?.GetValue<int>()).ShouldBe(4);
    }
    
    [Fact]
    public async Task RemoveOrderProduct_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/orders";
        
        // create order
        var orderInfo = await _client.CreateFullOrder();
        var productIdA = await _client.CreateProduct("another product a", "product a description", "SK0001A");
        var productIdB = await _client.CreateProduct("another product b", "product b description", "SK0001B");

        // add order products
        var response = await _client.PostAsJsonAsync($"{url}/{orderInfo.OrderId}/products", new
        {
            quantity = 5,
            productId = productIdA
        });
        response.EnsureSuccessStatusCode();
        response = await _client.PostAsJsonAsync($"{url}/{orderInfo.OrderId}/products", new
        {
            quantity = 2,
            productId = productIdB
        });
        response.EnsureSuccessStatusCode();
        response = await _client.PostAsJsonAsync($"{url}/{orderInfo.OrderId}/products", new
        {
            quantity = 3,
            productId = productIdB
        });
        response.EnsureSuccessStatusCode();
        
        // Act
        response = await _client.DeleteAsJsonAsync($"{url}/{orderInfo.OrderId}/products", new
        {
            quantity = 5,
            productId = productIdB
        });
        response.EnsureSuccessStatusCode();
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        response = await _client.GetAsync($"{url}/{orderInfo.OrderId}/products");
        response.EnsureSuccessStatusCode();
        var orderProducts = await response.Content.ReadFromJsonAsync<JsonArray>();
        orderProducts.ShouldNotBeNull();
        orderProducts.ShouldNotBeNull();
        orderProducts.Count.ShouldBe(2);
        orderProducts.Sum(node => node?["quantity"]?.GetValue<int>()).ShouldBe(6);
    }
    
    [Fact]
    public async Task GetOrders_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/orders";
        
        // create orders
        var orderInfo1 = await _client.CreateFullOrder();
        
        var orderInfo2 = await _client.CreateFullOrder();
        
        var orderInfo3 = await _client.CreateFullOrder();

        var response = await _client.PostAsJsonAsync($"{url}/{orderInfo3.OrderId}/process", new { });
        response.EnsureSuccessStatusCode();

        // Act
        response = await _client.GetAsync($"{url}?customerId={orderInfo1.CustomerId}");
        response.EnsureSuccessStatusCode();
        
        // Assert
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["orders"].ShouldNotBeNull();
        var orders = content["orders"]!.AsArray();
        orders.Count.ShouldBe(1);
        orders[0]?["customerId"]?.ToString().ShouldBe(orderInfo1.CustomerId.ToString());
        orders[0]?["orderId"]?.ToString().ShouldBe(orderInfo1.OrderId.ToString());
        orders[0]?["status"]?.ToString().ShouldBe("Pending");
        
        response = await _client.GetAsync($"{url}?status=Pending");
        response.EnsureSuccessStatusCode();
        content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["orders"].ShouldNotBeNull();
        orders = content["orders"]!.AsArray();
        orders.Count.ShouldBeGreaterThanOrEqualTo(2);

        var order1 = orders.Single(o => o?["id"]?.GetValue<Guid>() == orderInfo1.OrderId);
        var order2 = orders.Single(o => o?["id"]?.GetValue<Guid>() == orderInfo2.OrderId);
        
        order1?["customerId"]?.ToString().ShouldBe(orderInfo1.CustomerId.ToString());
        order1?["status"]?.ToString().ShouldBe("Pending");
        order2?["customerId"]?.ToString().ShouldBe(orderInfo2.CustomerId.ToString());
        order2?["status"]?.ToString().ShouldBe("Pending");
    }
}