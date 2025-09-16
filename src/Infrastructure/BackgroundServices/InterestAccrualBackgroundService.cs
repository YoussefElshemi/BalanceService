using System.Diagnostics;
using Core.Configs;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundServices;

public class InterestAccrualBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<AppConfig> appConfig) : BackgroundService
{
    private readonly InterestAccrualJob _interestAccrualJob = appConfig.Value.InterestAccrualJob;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _interestAccrualJob.Enabled)
        {
            using var currentActivity = OpenTelemetry.OpenTelemetry.MyActivitySource.StartActivity(GetType().Name);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var accrualService = scope.ServiceProvider.GetRequiredService<IInterestAccrualService>();

                await accrualService.AccrueMissingDaysAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                currentActivity?.AddException(exception);
                currentActivity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            }
            finally
            {
                currentActivity?.Stop();
                await Task.Delay(_interestAccrualJob.Interval, cancellationToken);
            }
        }
    }
}