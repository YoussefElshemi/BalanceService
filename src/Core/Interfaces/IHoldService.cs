using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IHoldService
{
    Task<Hold> CreateAsync(CreateHoldRequest createHoldRequest, CancellationToken cancellationToken);
    Task<Hold> GetByIdAsync(HoldId holdId, CancellationToken cancellationToken);
    Task ReleaseAsync(HoldId holdId, Username releasedBy, CancellationToken cancellationToken);
    Task DeleteAsync(HoldId holdId, Username deletedBy, CancellationToken cancellationToken);
    Task<Transaction> SettleAsync(HoldId holdId, Username settledBy, CancellationToken cancellationToken);
    Task<Hold> UpdateAsync(UpdateHoldRequest updateHoldRequest, CancellationToken cancellationToken);
    Task<PagedResults<Hold>> QueryAsync(QueryHoldsRequest queryHoldsRequest, CancellationToken cancellationToken);
    Task ExpireHoldsAsync(CancellationToken cancellationToken);
    Task<PagedResults<ChangeEvent>> GetHistoryAsync(GetHistoryRequest getHistoryRequest, CancellationToken cancellationToken);
}