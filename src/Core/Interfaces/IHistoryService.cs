using Core.Models;

namespace Core.Interfaces;

public interface IHistoryService<TModel>
{
    Task<int> CountChangesAsync(GetChangesRequest getChangesRequest, CancellationToken cancellationToken);
    Task<List<ChangeEvent>> GetChangesAsync(GetChangesRequest getChangesRequest, CancellationToken cancellationToken);
}