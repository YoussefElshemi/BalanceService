using Core.Enums;
using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IAccountRepository
{
    Task CreateAsync(Account account, CancellationToken cancellationToken);
    Task<List<Account>> QueryAsync(QueryAccountsRequest queryAccountsRequest, CancellationToken cancellationToken);
    Task<int> CountAsync(QueryAccountsRequest queryAccountsRequest, CancellationToken cancellationToken);
    Task<Account?> GetByIdAsync(AccountId accountId, CancellationToken cancellationToken);
    Task<List<Account>> GetByIdsAsync(List<AccountId> accountIds, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(AccountId accountId, CancellationToken cancellationToken);
    Task<Account> UpdateAsync(UpdateAccountRequest updateAccountRequest, CancellationToken cancellationToken);
    Task DeleteAsync(AccountId accountId, Username deletedBy, CancellationToken cancellationToken);
    Task<AccountBalance?> GetBalancesByIdAsync(AccountId accountId, CancellationToken cancellationToken);
    Task UpdateStatusAsync(UpdateAccountStatusRequest updateAccountStatusRequest, CancellationToken cancellationToken);
    Task<AccountStatus?> GetStatusByIdAsync(AccountId accountId, CancellationToken cancellationToken);
}