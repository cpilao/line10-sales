using Testcontainers.PostgreSql;

namespace Line10.Sales.IntegrationTests.Infrastructure;

public class PostgreSqlContainerWrapper : IAsyncDisposable
{
    private readonly PostgreSqlContainer _container;

    public PostgreSqlContainerWrapper()
    {
        _container = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("Password123!")
            .Build();
    }

    public string GetConnectionString() => _container.GetConnectionString();

    public async Task StartAsync()
    {
        await _container.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
    }
}