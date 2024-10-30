using Kronos.WebAPI.Athena.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Kronos.WebAPI;

public class AutoEfMigrationsHostedService(IServiceProvider serviceProvider) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
// Apply migrations automatically
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AthenaDbContext>();
            await context.Database.MigrateAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }
}