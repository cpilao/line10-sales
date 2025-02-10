using Line10.Sales.Domain.Persistence;
using Line10.Sales.Infrastructure;
using Line10.Sales.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Line10.Sales.IntegrationTests.Infrastructure;

public class IntegrationDataTestFixture: IAsyncLifetime
{
    public PostgreSqlContainerWrapper DataContainer { get; private set; } = null!;
    public ApplicationDbContext DbContext { get; private set; } = null!;
    public ICustomerRepository CustomerRepository { get; private set; } = null!;
    public IProductRepository ProductRepository { get; private set; } = null!;
    public IOrderRepository OrderRepository { get; private set; } = null!;
    
    public async Task InitializeAsync()
    {
        DataContainer = new PostgreSqlContainerWrapper();
        await DataContainer.StartAsync();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(DataContainer.GetConnectionString())
            .Options;

        DbContext = new ApplicationDbContext(options);
        await DbContext.Database.MigrateAsync();

        CustomerRepository = new CustomerRepository(DbContext);
        ProductRepository = new ProductRepository(DbContext);
        OrderRepository = new OrderRepository(DbContext);
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await DataContainer.DisposeAsync();
    }
}