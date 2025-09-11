using System.Diagnostics;
using Core.Configs;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundServices;

public class HoldExpiryBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<AppConfig> appConfig) : BackgroundService
{
    private readonly HoldExpiryConfig _holdExpiryConfig = appConfig.Value.HoldExpiryConfig;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (!_holdExpiryConfig.Enabled)
        {
            return;
        }

        using var timer = new PeriodicTimer(_holdExpiryConfig.Interval);

        do
        {
            var currentActivity = OpenTelemetry.OpenTelemetry.MyActivitySource.StartActivity(GetType().Name);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var holdService = scope.ServiceProvider.GetRequiredService<IHoldService>();

                await holdService.ExpireHoldsAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                currentActivity?.AddException(exception);
                currentActivity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            }
            finally
            {
                currentActivity?.Stop();
            }
        } while (await timer.WaitForNextTickAsync(cancellationToken));
    }
}