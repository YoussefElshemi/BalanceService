using System.Diagnostics;
using Core.Configs;
using Core.Models;
using Infrastructure.Entities;
using Infrastructure.Entities.History;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Infrastructure.BackgroundServices;

public class UpdateNotificationBackgroundService<TEntity, TModel, TKey, TConfig, TDto>(
    IServiceScopeFactory scopeFactory,
    IOptions<TConfig> configOptions) : BackgroundService
    where TEntity : class, IHistoryEntity<TModel>
    where TModel : IHistory<TKey, TDto>
    where TKey : IEquatable<TKey>
    where TConfig : class, IUpdateNotificationConfig
    where TDto : class
{
    private readonly TConfig _config = configOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _config.Enabled)
        {
            using var activity = OpenTelemetry.OpenTelemetry.MyActivitySource.StartActivity(GetType().Name);

            try
            {
                using var scope = scopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<IHistoryUpdateProcessor<TEntity, TModel, TKey, TConfig, TDto>>();
                await processor.ProcessAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                activity?.AddException(ex);
                activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            }
            finally
            {
                activity?.Stop();
                await Task.Delay(_config.Interval, cancellationToken);
            }
        }
    }
}