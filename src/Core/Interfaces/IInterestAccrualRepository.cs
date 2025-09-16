using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IInterestAccrualRepository
{
    Task CreateAsync(InterestAccrual interestAccrual, CancellationToken cancellationToken);
    Task<AccruedAt?> GetLastAccrualDateAsync(CancellationToken cancellationToken);
    Task<bool> ExistsByDateAsync(DateOnly date, CancellationToken cancellationToken);
    Task PostAsync(InterestAccrualId interestAccrualId, Username postedBy, CancellationToken cancellationToken);
    Task<List<InterestAccrual>> GetUnpostedAsync(AccountId accountId, InterestProductId interestProductId, CancellationToken cancellationToken);
}