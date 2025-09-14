using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IHoldRepository
{
    Task CreateAsync(Hold hold, CancellationToken cancellationToken);
    Task<Hold?> GetByIdAsync(HoldId holdId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(HoldId holdId, CancellationToken cancellationToken);
    Task ReleaseAsync(HoldId holdId, Username releasedBy, CancellationToken cancellationToken);
    Task DeleteAsync(HoldId holdId, Username deletedBy, CancellationToken cancellationToken);
    Task SettleAsync(HoldId holdId, TransactionId transactionId, Username settledBy, CancellationToken cancellationToken);
    Task<Hold> UpdateAsync(HoldId holdId, UpdateHoldRequest updateHoldRequest, CancellationToken cancellationToken);
    Task<int> CountAsync(QueryHoldsRequest queryHoldsRequest, CancellationToken cancellationToken);
    Task<List<Hold>> QueryAsync(QueryHoldsRequest queryHoldsRequest, CancellationToken cancellationToken);
    Task ExpireHoldsAsync(CancellationToken cancellationToken);
}