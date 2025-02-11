using Line10.Sales.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Line10.Sales.IntegrationTests.Api;

public class IntegrationApiTestFixture :
    WebApplicationFactory<Program>,
    IAsyncLifetime
{
    private PostgreSqlContainerWrapper DataContainer { get; set; } = null!;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // Override the connection string here
            var testConfiguration = new Dictionary<string, string>
            {
                { "ConnectionStrings:DefaultConnection", DataContainer.GetConnectionString() }
            };

            config.AddInMemoryCollection(testConfiguration!);
        });

        return base.CreateHost(builder);
    }
    
    public async Task InitializeAsync()
    {
        DataContainer = new PostgreSqlContainerWrapper();
        await DataContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}