using System.Diagnostics;
using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Core.Configs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Entities.History;
using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

public class HistoryUpdateProcessor<TEntity, TModel, TDto, TConfig>(
    IAmazonSimpleNotificationService amazonSns,
    IHistoryRepository<TEntity, TModel> repository,
    IUnitOfWork unitOfWork,
    IOptions<TConfig> configOptions) : IHistoryUpdateProcessor<TEntity, TModel, TDto, TConfig>
    where TEntity : class, IHistoryEntity<TModel>
    where TModel : IHistory<TDto>
    where TDto : class
    where TConfig : class, IUpdateNotificationConfig
{
    private readonly TConfig _config = configOptions.Value;

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        var models = await repository.GetPendingAsync(_config.MaxCount, cancellationToken);

        if (models.Count == 0)
        {
            return;
        }

        foreach (var model in models)
        {
            using var currentActivity = OpenTelemetry.OpenTelemetry.MyActivitySource.StartActivity($"{GetType().Name}-{model.GetPrimaryKey()}");

            try
            {
                await ProcessUpdateAsync(model, cancellationToken);
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
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task ProcessUpdateAsync(TModel model, CancellationToken cancellationToken)
    {
        await amazonSns.PublishAsync(new PublishRequest
        {
            TopicArn = _config.TopicArn,
            Message = JsonSerializer.Serialize(model.ToDto())
        }, cancellationToken);

        await repository.MarkAsProcessedAsync(model.GetPrimaryKey(), cancellationToken);
    }
}
