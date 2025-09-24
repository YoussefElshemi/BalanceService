using Core.Configs;
using Core.Models;
using Infrastructure.Entities;

namespace Infrastructure.Services.Interfaces;

public interface IHistoryUpdateProcessor<TEntity, TModel, TDto, TConfig>
    where TEntity : class, IHistoryEntity<TModel>
    where TModel : IHistory<TDto>
    where TDto : class
    where TConfig : class, IUpdateNotificationConfig
{
    Task ProcessAsync(CancellationToken cancellationToken);
}