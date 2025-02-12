using Line10.Sales.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Line10.Sales.Api.BackgroundServices;

public class MigrationService : BackgroundService
{
    private readonly ILogger<MigrationService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MigrationService(
        ILogger<MigrationService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync(stoppingToken);
                break;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error on database migrations");
                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}