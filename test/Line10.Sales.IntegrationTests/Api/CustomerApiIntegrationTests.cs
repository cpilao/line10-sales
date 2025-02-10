using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;

namespace Line10.Sales.IntegrationTests.Api;

public class CustomerApiIntegrationTests: IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CustomerApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateCustomer_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/customers";

        // Act
        var response = await _client.PostAsJsonAsync(url, new
        {
            firstName = "John",
            lastName = "Doe",
            email = "john.doe@example.com",
            phone = "123-456-7890"
        });
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["customerId"].ShouldNotBeNull();
        var customerId = content["customerId"]?.GetValue<Guid>();
        customerId.ShouldNotBe(Guid.Empty);
    }
    
    [Fact]
    public async Task GetCustomer_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/customers";

        // Act
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
        
        // Assert
        response = await _client.GetAsync($"{url}/{customerId}");
        response.EnsureSuccessStatusCode();
        
        content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["customerId"]?.ToString().ShouldBe(customerId.ToString());
        content["firstName"]?.ToString().ShouldBe("John");
        content["lastName"]?.ToString().ShouldBe("Doe");
        content["email"]?.ToString().ShouldBe("john.doe@example.com");
        content["phone"]?.ToString().ShouldBe("123-456-7890");
    }
    
    [Fact]
    public async Task DeleteCustomer_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/customers";

        // Act
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
        
        // Assert
        response = await _client.DeleteAsync($"{url}/{customerId}");
        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task UpdateCustomer_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/customers";

        // Act
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
        
        // Assert
        response = await _client.PutAsJsonAsync($"{url}/{customerId}",new
        {
            firstName = "Charles",
            lastName = "Smith",
            email = "charles.smith@example.com"
        });
        response.EnsureSuccessStatusCode();
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        response = await _client.GetAsync($"{url}/{customerId}");
        response.EnsureSuccessStatusCode();
        
        content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["customerId"]?.ToString().ShouldBe(customerId.ToString());
        content["firstName"]?.ToString().ShouldBe("Charles");
        content["lastName"]?.ToString().ShouldBe("Smith");
        content["email"]?.ToString().ShouldBe("charles.smith@example.com");
        content["phone"]?.ToString().ShouldBe("123-456-7890");
    }
}