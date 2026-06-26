namespace WrkPlan.CustomerPortal.API.Services;

public class ReminderBackgroundService(ILogger<ReminderBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Running recurring reminder scan for renewals and payment methods at {UtcNow}", DateTime.UtcNow);
            await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
        }
    }
}
