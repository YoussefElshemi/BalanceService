namespace Core.Interfaces;

public interface IHistoryRepository<TEntity, TModel, TKey>
{
    Task<List<TModel>> GetPendingAsync(int count, CancellationToken cancellationToken);
    Task MarkAsProcessedAsync(TKey primaryKey, CancellationToken cancellationToken);
}