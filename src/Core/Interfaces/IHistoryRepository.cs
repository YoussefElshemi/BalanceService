using Core.Enums;
using Core.Models;

namespace Core.Interfaces;

public interface IHistoryRepository<TEntity, TModel>
{
    Task<int> CountChangesAsync(GetHistoryRequest getHistoryRequest, CancellationToken cancellationToken);
    Task<List<ChangeEvent>> GetChangesAsync(GetHistoryRequest getHistoryRequest, CancellationToken cancellationToken);
    Task<List<TModel>> GetPendingAsync(int count, CancellationToken cancellationToken);
    Task UpdateProcessingStatusAsync(Guid primaryKey, ProcessingStatus processingStatus, CancellationToken cancellationToken);
    Task MarkAsProcessedAsync(Guid primaryKey, CancellationToken cancellationToken);
}