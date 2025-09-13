namespace Core.Interfaces;

public interface IHistoryRepository<TEntity, TModel>
{
    Task<List<TModel>> GetPendingAsync(int count, CancellationToken cancellationToken);
    Task MarkAsProcessedAsync(Guid primaryKey, CancellationToken cancellationToken);
}