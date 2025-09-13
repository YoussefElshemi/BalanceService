using Core.Configs;
using Core.Models;
using Infrastructure.Entities;

namespace Infrastructure.Services.Interfaces;

public interface IHistoryUpdateProcessor<TEntity, TModel, TKey, TConfig, TDto>
    where TEntity : class, IHistoryEntity<TModel>
    where TModel : IHistory<TKey, TDto>
    where TKey : IEquatable<TKey>
    where TConfig : class, IUpdateNotificationConfig
    where TDto : class
{
    Task ProcessAsync(CancellationToken cancellationToken);
}