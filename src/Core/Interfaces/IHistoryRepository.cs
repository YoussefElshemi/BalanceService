using Core.Enums;

namespace Core.Interfaces;

public interface IHistoryRepository<TEntity, TModel>
{
    Task<List<TModel>> GetPendingAsync(int count, CancellationToken cancellationToken);
    Task UpdateProcessingStatusAsync(Guid primaryKey, ProcessingStatus processingStatus, CancellationToken cancellationToken);
    Task MarkAsProcessedAsync(Guid primaryKey, CancellationToken cancellationToken);
}