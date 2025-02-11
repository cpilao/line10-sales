using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Shouldly;

namespace Line10.Sales.IntegrationTests.Extensions;

public static class HttpClientExtensions
{
    public static async Task<Guid> CreateCustomer(
        this HttpClient httpClient,
        string firstName,
        string lastName,
        string email,
        string phone)
    {
        var url = "/customers";
        var response = await httpClient.PostAsJsonAsync(url, new
        {
            firstName,
            lastName,
            email,
            phone
        });
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        var customerId = content["customerId"]?.GetValue<Guid>();
        customerId.ShouldNotBeNull();
        return customerId.Value;
    }
    
    public static async Task<Guid> CreateProduct(
        this HttpClient httpClient,
        string name,
        string description,
        string sku)
    {
        var url = "/products";
        var response = await httpClient.PostAsJsonAsync(url, new
        {
            name,
            description,
            sku
        });
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        var productId = content["productId"]?.GetValue<Guid>();
        productId.ShouldNotBeNull();
        return productId.Value;
    }
    
    public static async Task<(Guid OrderId, Guid CustomerId, Guid ProductId)> CreateFullOrder(
        this HttpClient httpClient)
    {
        var url = "/orders";
        
        // create customer
        var customerId = await httpClient.CreateCustomer(
            "John",
            "Doe",
            "john.doe@example.com",
            "123-456-7890");
        // create product
        var productId = await httpClient.CreateProduct(
            "product1",
            "product 1 description",
            "SK00001");
        // create order
        var response = await httpClient.PostAsJsonAsync(url, new
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
    
    public static async Task<HttpResponseMessage> DeleteAsJsonAsync(
        this HttpClient client,
        string requestUri, 
        object body)
    {
        var jsonBody = JsonSerializer.Serialize(body);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Delete, requestUri)
        {
            Content = content 
        };

        return await client.SendAsync(request);
    }
}