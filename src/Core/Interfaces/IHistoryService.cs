using Core.Models;

namespace Core.Interfaces;

public interface IHistoryService<TModel>
{
    Task<int> CountChangesAsync(GetHistoryRequest getHistoryRequest, CancellationToken cancellationToken);
    Task<List<ChangeEvent>> GetChangesAsync(GetHistoryRequest getHistoryRequest, CancellationToken cancellationToken);
}