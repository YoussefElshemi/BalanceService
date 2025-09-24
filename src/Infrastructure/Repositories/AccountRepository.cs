using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Extensions;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AccountRepository(
    ApplicationDbContext dbContext,
    TimeProvider timeProvider) : IAccountRepository
{
    public async Task CreateAsync(Account account, CancellationToken cancellationToken)
    {
        var accountEntity = account.ToEntity();
        await dbContext.Accounts.AddAsync(accountEntity, cancellationToken);
    }

    public async Task<List<Account>> QueryAsync(QueryAccountsRequest queryAccountsRequest, CancellationToken cancellationToken)
    {
        var query = dbContext.Accounts.BuildSearchQuery(queryAccountsRequest);

        var entities = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.AccountId)
            .Skip((queryAccountsRequest.PageNumber - 1) * queryAccountsRequest.PageSize)
            .Take(queryAccountsRequest.PageSize)
            .ToListAsync(cancellationToken);

        return entities.Select(x => x.ToModel()).ToList();
    }

    public Task<int> CountAsync(QueryAccountsRequest queryAccountsRequest, CancellationToken cancellationToken)
    {
        var query = dbContext.Accounts.BuildSearchQuery(queryAccountsRequest);

        return query.CountAsync(cancellationToken);
    }

    public async Task<Account?> GetByIdAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        var accountEntity = await dbContext.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.AccountId == accountId && x.IsDeleted == false, cancellationToken);

        return accountEntity?.ToModel();
    }

    public async Task<List<Account>> GetByIdsAsync(List<AccountId> accountIds, CancellationToken cancellationToken)
    {
        var ids = accountIds.Select(x => (Guid)x).ToHashSet();

        var accountEntities = await dbContext.Accounts
            .AsNoTracking()
            .Where(x => ids.Contains(x.AccountId) && x.IsDeleted == false)
            .ToListAsync(cancellationToken);

        return accountEntities.Select(x => x.ToModel()).ToList();
    }

    public Task<bool> ExistsAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        return dbContext.Accounts
            .AsNoTracking()
            .AnyAsync(x => x.AccountId == accountId && x.IsDeleted == false, cancellationToken);
    }

    public async Task<Account> UpdateAsync(UpdateAccountRequest updateAccountRequest, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var accountEntity = await dbContext.Accounts
            .AsTracking()
            .FirstAsync(x => x.AccountId == updateAccountRequest.AccountId, cancellationToken);

        accountEntity.ParentAccountId = updateAccountRequest.ParentAccountId;
        accountEntity.AccountName = updateAccountRequest.AccountName;
        accountEntity.AccountTypeId = (int)updateAccountRequest.AccountType;
        accountEntity.Metadata = updateAccountRequest.Metadata?.RootElement.GetRawText();
        accountEntity.CurrencyCode  = updateAccountRequest.CurrencyCode.ToString();
        accountEntity.MinimumRequiredBalance = updateAccountRequest.MinimumRequiredBalance;
        accountEntity.UpdatedBy = updateAccountRequest.UpdatedBy;
        accountEntity.UpdatedAt = utcDateTime;

        return accountEntity.ToModel();
    }

    public async Task DeleteAsync(AccountId accountId, Username deletedBy,CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var accountEntity = await dbContext.Accounts
            .AsTracking()
            .FirstAsync(x => x.AccountId == accountId, cancellationToken);

        accountEntity.IsDeleted = true;
        accountEntity.DeletedBy = deletedBy;
        accountEntity.DeletedAt = utcDateTime;
    }

    public Task<AccountBalance?> GetBalancesByIdAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        return dbContext.Accounts
            .AsNoTracking()
            .Where(x => x.AccountId == accountId && x.IsDeleted == false)
            .Select(x => new AccountBalance
            {
                AccountId = accountId,
                AvailableBalance = new AvailableBalance(x.AvailableBalance),
                LedgerBalance = new LedgerBalance(x.LedgerBalance),
                PendingDebitBalance = new PendingDebitBalance(x.PendingDebitBalance),
                PendingCreditBalance = new PendingCreditBalance(x.PendingCreditBalance),
                HoldBalance = new HoldBalance(x.HoldBalance),
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(UpdateAccountStatusRequest updateAccountStatusRequest, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var accountEntity = await dbContext.Accounts
            .AsTracking()
            .FirstAsync(x => x.AccountId == updateAccountStatusRequest.AccountId, cancellationToken);

        accountEntity.AccountStatusId = (int)updateAccountStatusRequest.AccountStatus;
        accountEntity.UpdatedBy = updateAccountStatusRequest.UpdatedBy;
        accountEntity.UpdatedAt = utcDateTime;
    }

    public async Task<AccountStatus?> GetStatusByIdAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        var statusId = await dbContext.Accounts
            .AsNoTracking()
            .Where(a => a.AccountId == accountId && a.IsDeleted == false)
            .Select(a => a.AccountStatusId)
            .FirstOrDefaultAsync(cancellationToken);

        return (AccountStatus)statusId;
    }
}
