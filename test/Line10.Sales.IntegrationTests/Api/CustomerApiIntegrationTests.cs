using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Line10.Sales.Core.Security;
using Line10.Sales.IntegrationTests.Extensions;
using Shouldly;

namespace Line10.Sales.IntegrationTests.Api;

public class CustomerApiIntegrationTests: BaseApiIntegrationTest
{
    private readonly HttpClient _client;
    
    public CustomerApiIntegrationTests(IntegrationApiTestFixture fixture)
        : base(fixture)
    {
        _client = _fixture.CreateClient();
        var token = JwtUtils.GetToken(["customers.read", "customers.write"]);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
    
    [Fact]
    public async Task GetCustomers_ShouldReturnSuccess()
    {
        // Arrange
        var url = "/customers";
        var customersCount = 100;
        var pageSize = 10;
        var pageNumber = 1;

        for (var i = 0; i < customersCount; i++)
        {
            var createCustomerResponse = await _client.PostAsJsonAsync(url, new
            {
                firstName = $"John_{i+1}",
                lastName = "Doe",
                email = "john.doe@example.com",
                phone = "123-456-7890"
            });
            createCustomerResponse.EnsureSuccessStatusCode();
        }

        // Act
        while (customersCount!=0)
        {
            var response = await _client.GetAsync($"{url}?pageSize={pageSize}&pageNumber={pageNumber}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadFromJsonAsync<JsonNode>();
            content.ShouldNotBeNull();
            content["customers"].ShouldNotBeNull();
            var customers = content["customers"]!.AsArray();
            customersCount -= customers.Count;
            if (customersCount != 0)
            {
                pageNumber++;
            }
        }
        
        // Assert
        pageNumber.ShouldBe(10);
    }
    
    [Theory]
    [InlineData("FirstName", "Desc", "customer_c", "customer_b", "customer_a")]
    [InlineData("FirstName", "Asc", "customer_a", "customer_b", "customer_c")]
    [InlineData("LastName", "Desc", "customer_c", "customer_a", "customer_b")]
    [InlineData("LastName", "Asc", "customer_b", "customer_a", "customer_c")]
    [InlineData("Email", "Asc", "customer_a", "customer_b", "customer_c")]
    [InlineData("Email", "Desc", "customer_c", "customer_b", "customer_a")]
    [InlineData("Phone", "Asc", "customer_a", "customer_b", "customer_c")]
    [InlineData("Phone", "Desc", "customer_c", "customer_b", "customer_a")]
    public async Task GetCustomers_ShouldReturnOrderedCustomers(
        string orderBy,
        string order,
        string expectedFirstCustomerName,
        string expectedSecondCustomerName,
        string expectedThirdCustomerName)
    {
        // Arrange
        var url = "/customers";
        var testId = Guid.NewGuid().ToString();
        
        await _client.CreateCustomer($"{testId}_customer_a", "customer last name b", "customer_a@example.com", "123-456-7890");
        await _client.CreateCustomer($"{testId}_customer_b", "customer last name a", "customer_b@example.com", "123-456-7891");
        await _client.CreateCustomer($"{testId}_customer_c", "customer last name c", "customer_c@example.com", "123-456-7892");
        
        // Act
        var response = await _client.GetAsync($"{url}?orderBy={orderBy}&order={order}&firstName={testId}");
        response.EnsureSuccessStatusCode();
        
        // Assert
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["customers"].ShouldNotBeNull();
        var products = content["customers"]!.AsArray();
        products.ShouldNotBeNull();
        products[0]?["firstName"]?.GetValue<string>().ShouldBe($"{testId}_{expectedFirstCustomerName}");
        products[1]?["firstName"]?.GetValue<string>().ShouldBe($"{testId}_{expectedSecondCustomerName}");
        products[2]?["firstName"]?.GetValue<string>().ShouldBe($"{testId}_{expectedThirdCustomerName}");
    }
    
    [Theory]
    [InlineData("FirstName", "fname_c", 1)]
    [InlineData("FirstName", "fname", 3)]
    [InlineData("LastName", "lname_b", 1)]
    [InlineData("LastName", "lname_x", 2)]
    public async Task GetCustomers_ShouldReturnFilteredCustomers(
        string filterName,
        string filterValue,
        int expectedResultsCount)
    {
        // Arrange
        var url = "/customers";
        var testId = Guid.NewGuid().ToString();
        
        await _client.CreateCustomer($"{testId}_fname_a", $"{testId}_lname_b", $"{testId}_a@e.com", $"123-456-7890");
        await _client.CreateCustomer($"{testId}_fname_b", $"{testId}_lname_x1 a", $"{testId}_b@e.com", $"123-456-7891");
        await _client.CreateCustomer($"{testId}_fname_c", $"{testId}_lname_x2 c", $"{testId}_c@e.com", $"123-456-7892");
        
        // Act
        var response = await _client.GetAsync($"{url}?{filterName}={testId}_{filterValue}");
        response.EnsureSuccessStatusCode();

        // Assert
        var content = await response.Content.ReadFromJsonAsync<JsonNode>();
        content.ShouldNotBeNull();
        content["customers"].ShouldNotBeNull();

        var products = content["customers"]!.AsArray();
        products.ShouldNotBeNull();
        products.Count.ShouldBe(expectedResultsCount);
    }
}