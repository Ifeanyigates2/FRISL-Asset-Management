using Microsoft.Extensions.DependencyInjection;

namespace FrislEams.Web.Services;

public class IntegrationWorker(IServiceScopeFactory scopeFactory, IIntegrationQueue queue, ILogger<IntegrationWorker> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var job = await queue.DequeueAsync(stoppingToken);
                using var scope = scopeFactory.CreateScope();
                var orchestrator = scope.ServiceProvider.GetRequiredService<IntegrationOrchestrator>();
                await orchestrator.ProcessAsync(job, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Integration worker failed processing a queued event");
            }
        }
    }
}
