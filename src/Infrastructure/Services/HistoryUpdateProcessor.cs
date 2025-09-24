using System.Diagnostics;
using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Core.Configs;
using Core.Constants;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Entities.History;
using Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;

namespace Infrastructure.Services;

public class HistoryUpdateProcessor<TEntity, TModel, TDto, TConfig>(
    IAmazonSimpleNotificationService amazonSns,
    IHistoryRepository<TEntity, TModel> historyRepository,
    IUnitOfWork unitOfWork,
    IOptions<TConfig> configOptions) : IHistoryUpdateProcessor<TEntity, TModel, TDto, TConfig>
    where TEntity : class, IHistoryEntity<TModel>
    where TModel : IHistory<TDto>
    where TDto : class
    where TConfig : class, IUpdateNotificationConfig
{
    private readonly TConfig _config = configOptions.Value;

    private readonly AsyncCircuitBreakerPolicy _circuitBreaker = Policy
        .Handle<Exception>(ex => ex is not DbUpdateConcurrencyException)
        .CircuitBreakerAsync(
            exceptionsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30)
        );

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        var models = await historyRepository.GetPendingAsync(_config.MaxCount, cancellationToken);

        if (models.Count == 0)
        {
            return;
        }

        foreach (var model in models)
        {
            using var currentActivity = OpenTelemetry.OpenTelemetry.MyActivitySource.StartActivity($"{GetType().Name}");
            currentActivity?.AddTag(OpenTelemetryTags.Service.HistoryPrimaryKey, model.GetPrimaryKey());
            currentActivity?.AddTag(OpenTelemetryTags.Service.HistoryKey, model.GetKey());

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
    }

    private async Task ProcessUpdateAsync(TModel model, CancellationToken cancellationToken)
    {
        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            await historyRepository.UpdateProcessingStatusAsync(
                model.GetPrimaryKey(),
                ProcessingStatus.Processing,
                ct);

            await unitOfWork.SaveChangesAsync(ct);

            await _circuitBreaker.ExecuteAsync(async () =>
            {
                await amazonSns.PublishAsync(new PublishRequest
                {
                    TopicArn = _config.TopicArn,
                    Message = JsonSerializer.Serialize(model.ToDto()),
                    MessageGroupId = "default",
                    MessageDeduplicationId = model.GetPrimaryKey().ToString()
                }, ct);
            });

            await historyRepository.MarkAsProcessedAsync(model.GetPrimaryKey(), ct);
            await unitOfWork.SaveChangesAsync(ct);
        }, cancellationToken);
    }
}
