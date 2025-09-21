using System.Diagnostics;
using Core.Constants;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;

namespace Core.Services;

public class AccountService(
    IAccountRepository accountRepository,
    IHistoryService<AccountHistory> accountHistoryService,
    IAccountRulesService accountRulesService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IAccountService
{
    public async Task<Account> CreateAsync(CreateAccountRequest createAccountRequest, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();

        var account = new Account
        {
            AccountId = new AccountId(Guid.NewGuid()),
            AccountName = createAccountRequest.AccountName,
            CurrencyCode = createAccountRequest.CurrencyCode,
            AvailableBalance = new AvailableBalance(0),
            LedgerBalance = new LedgerBalance(0),
            PendingBalance = new PendingBalance(0),
            HoldBalance = new HoldBalance(0),
            MinimumRequiredBalance = createAccountRequest.MinimumRequiredBalance,
            Type = createAccountRequest.AccountType,
            Status = AccountStatus.PendingActivation,
            Metadata = createAccountRequest.Metadata,
            ParentAccountId = createAccountRequest.ParentAccountId,
            CreatedAt = new CreatedAt(utcDateTime),
            CreatedBy = createAccountRequest.CreatedBy,
            UpdatedAt = new UpdatedAt(utcDateTime),
            UpdatedBy = createAccountRequest.CreatedBy,
            IsDeleted = false,
            DeletedAt = null,
            DeletedBy = null,
        };

        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, account.AccountId.ToString());

        await accountRepository.CreateAsync(account, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return account;
    }

    public async Task<PagedResults<Account>> QueryAsync(QueryAccountsRequest queryAccountsRequest, CancellationToken cancellationToken)
    {
        var count = await accountRepository.CountAsync(queryAccountsRequest, cancellationToken);
        var accounts = await accountRepository.QueryAsync(queryAccountsRequest, cancellationToken);

        return new PagedResults<Account>
        {
            Data = accounts,
            MetaData = new PagedMetadata
            {
                TotalRecords = count,
                TotalPages = (count + queryAccountsRequest.PageSize - 1) / queryAccountsRequest.PageSize,
                PageSize = queryAccountsRequest.PageSize,
                PageNumber = queryAccountsRequest.PageNumber
            }
        };
    }

    public async Task<Account> GetByIdAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, accountId.ToString());

        var account = await accountRepository.GetByIdAsync(accountId, cancellationToken) 
                      ?? throw new NotFoundException();

        return account;
    }

    public async Task<List<Account>> GetByIdsAsync(List<AccountId> accountIds, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, string.Join(", ", accountIds.Select(x => x.ToString())));

        var accounts = await accountRepository.GetByIdsAsync(accountIds, cancellationToken);

        return accounts;
    }

    public Task<bool> ExistsAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, accountId.ToString());

        return accountRepository.ExistsAsync(accountId, cancellationToken);
    }

    public async Task<Account> UpdateAsync(UpdateAccountRequest updateAccountRequest, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, updateAccountRequest.AccountId.ToString());

        if (!await accountRepository.ExistsAsync(updateAccountRequest.AccountId, cancellationToken))
        {
            throw new NotFoundException();
        }

        if (updateAccountRequest.AccountId == updateAccountRequest.ParentAccountId)
        {
            throw new UnprocessableRequestException($"{nameof(AccountId)} must not match {nameof(UpdateAccountRequest.ParentAccountId)}");
        }

        var account = await accountRepository.UpdateAsync(updateAccountRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return account;
    }

    public async Task DeleteAsync(AccountId accountId, Username deletedBy, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, accountId.ToString());

        await accountRulesService.ThrowIfNotAllowedAsync(accountId, AccountOperationType.DeleteAccount, cancellationToken);

        await accountRepository.DeleteAsync(accountId, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<AccountBalance> GetBalancesByIdAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, accountId.ToString());

        var accountBalances = await accountRepository.GetBalancesByIdAsync(accountId, cancellationToken)
                              ?? throw new NotFoundException();

        return accountBalances;
    }

    public async Task ActivateAsync(AccountId accountId, Username activatedBy, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, accountId.ToString());

        await accountRulesService.ThrowIfNotAllowedAsync(accountId, AccountOperationType.ActivateAccount, cancellationToken);

        var updateAccountStatusRequest = new UpdateAccountStatusRequest
        {
            AccountId = new AccountId(accountId),
            AccountStatus = AccountStatus.Active,
            UpdatedBy = new Username(activatedBy)
        };

        await UpdateStatusAsync(updateAccountStatusRequest, cancellationToken);
    }

    public async Task FreezeAsync(AccountId accountId, Username frozenBy, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, accountId.ToString());

        await accountRulesService.ThrowIfNotAllowedAsync(accountId, AccountOperationType.FreezeAccount, cancellationToken);

        var updateAccountStatusRequest = new UpdateAccountStatusRequest
        {
            AccountId = new AccountId(accountId),
            AccountStatus = AccountStatus.Active,
            UpdatedBy = new Username(frozenBy)
        };

        await UpdateStatusAsync(updateAccountStatusRequest, cancellationToken);
    }

    public async Task CloseAsync(AccountId accountId, Username closedBy, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, accountId.ToString());

        var account = await GetByIdAsync(accountId, cancellationToken);

        accountRulesService.ThrowIfNotAllowed(account.Status, AccountOperationType.CloseAccount);

        List<decimal> balances = [
            account.AvailableBalance,
            account.LedgerBalance,
            account.PendingBalance,
            account.HoldBalance
        ];

        var updateAccountStatusRequest = new UpdateAccountStatusRequest
        {
            AccountId = new AccountId(accountId),
            UpdatedBy = new Username(closedBy),
            AccountStatus = balances.All(x => x == 0) 
                ? AccountStatus.Closed 
                : AccountStatus.PendingClosure
        };

        await UpdateStatusAsync(updateAccountStatusRequest, cancellationToken);
    }

    public async Task<PagedResults<ChangeEvent>> GetHistoryAsync(GetHistoryRequest getHistoryRequest, CancellationToken cancellationToken)
    {
        if (!await accountRepository.ExistsAsync(new AccountId(getHistoryRequest.EntityId), cancellationToken))
        {
            throw new NotFoundException();
        }
        
        var count = await accountHistoryService.CountChangesAsync(getHistoryRequest, cancellationToken);
        var changeEvents = await accountHistoryService.GetChangesAsync(getHistoryRequest, cancellationToken);

        return new PagedResults<ChangeEvent>
        {
            Data = changeEvents,
            MetaData = new PagedMetadata
            {
                TotalRecords = count,
                TotalPages = (count + getHistoryRequest.PageSize - 1) / getHistoryRequest.PageSize,
                PageSize = getHistoryRequest.PageSize,
                PageNumber = getHistoryRequest.PageNumber
            }
        };
    }

    private async Task UpdateStatusAsync(UpdateAccountStatusRequest updateAccountStatusRequest, CancellationToken cancellationToken)
    {
        await accountRepository.UpdateStatusAsync(updateAccountStatusRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}