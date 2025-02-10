using Line10.Sales.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Line10.Sales.Api.BackgroundServices;

public class MigrationService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MigrationService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}