using System.Diagnostics;
using Core.Configs;
using Core.Constants;
using Core.Interfaces;
using Core.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using CronExpression = Cronos.CronExpression;

namespace Infrastructure.BackgroundServices;

public class JobBackgroundService<TConfig, TExecutor>(
    IServiceScopeFactory scopeFactory,
    IOptions<TConfig> configOptions,
    TimeProvider timeProvider)
    : BackgroundService
    where TConfig : class, IJobConfig
    where TExecutor : class, IJobExecutor
{
    private readonly TConfig _config = configOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (!_config.Enabled)
        {
            return;
        }

        using var scope = scopeFactory.CreateScope();
        var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();

        var job = await jobService.GetOrCreateAsync(_config, cancellationToken);
        var cronSchedule = CronExpression.Parse(_config.CronExpression);

        while (!cancellationToken.IsCancellationRequested)
        {
            var utcNow = timeProvider.GetUtcNow();
            var nextOccurrence = cronSchedule.GetNextOccurrence(utcNow.UtcDateTime, TimeZoneInfo.Utc);

            if (!nextOccurrence.HasValue)
            {
                throw new InvalidOperationException("Could not calculate the next occurrence from the cron schedule.");
            }

            var delay = nextOccurrence.Value - utcNow.UtcDateTime;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                break;
            }

            using var currentActivity = OpenTelemetry.OpenTelemetry.MyActivitySource.StartActivity(_config.JobName);
            currentActivity?.AddTag(OpenTelemetryTags.Service.JobScheduledAt, nextOccurrence.Value);

            var scheduledAt = new ScheduledAt(TimeZoneInfo.ConvertTime(nextOccurrence.Value, TimeZoneInfo.Utc));
            var (success, jobRun) = await jobService.TryCreateRunAsync(job.JobId, scheduledAt, cancellationToken);
            currentActivity?.AddTag(OpenTelemetryTags.Service.JobRunCreated, success);

            if (!success)
            {
                continue;
            }

            try
            {
                var jobExecutor = scope.ServiceProvider.GetRequiredService<TExecutor>();
                await jobExecutor.ExecuteAsync(cancellationToken);
                await jobService.ExecuteAsync(jobRun.JobRunId, SystemConstants.Username, cancellationToken);

                currentActivity?.AddTag(OpenTelemetryTags.Service.JobSuccess, true);
            }
            catch (Exception exception)
            {
                await jobService.DeleteAsync(jobRun.JobRunId, SystemConstants.Username, cancellationToken);

                currentActivity?.AddException(exception);
                currentActivity?.SetStatus(ActivityStatusCode.Error, exception.Message);
                currentActivity?.AddTag(OpenTelemetryTags.Service.JobSuccess, false);
            }
            finally
            {
                currentActivity?.Stop();
            }
        }
    }
}
