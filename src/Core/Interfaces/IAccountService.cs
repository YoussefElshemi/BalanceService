using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IAccountService
{
    Task<Account> CreateAsync(CreateAccountRequest createAccountRequest, CancellationToken cancellationToken);
    Task<PagedResults<Account>> QueryAsync(QueryAccountsRequest queryAccountsRequest, CancellationToken cancellationToken);
    Task<Account> GetByIdAsync(AccountId accountId, CancellationToken cancellationToken);
    Task<List<Account>> GetByIdsAsync(List<AccountId> accountIds, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(AccountId accountId, CancellationToken cancellationToken);
    Task<Account> UpdateAsync(UpdateAccountRequest updateAccountRequest, CancellationToken cancellationToken);
    Task DeleteAsync(AccountId accountId, Username deletedBy, CancellationToken cancellationToken);
    Task<AccountBalance> GetBalancesByIdAsync(AccountId accountId, CancellationToken cancellationToken);
    Task ActivateAsync(AccountId accountId, Username activatedBy, CancellationToken cancellationToken);
    Task FreezeAsync(AccountId accountId, Username frozenBy, CancellationToken cancellationToken);
    Task CloseAsync(AccountId accountId, Username closedBy, CancellationToken cancellationToken);
    Task<PagedResults<ChangeEvent>> GetHistoryAsync(GetHistoryRequest getHistoryRequest, CancellationToken cancellationToken);
}