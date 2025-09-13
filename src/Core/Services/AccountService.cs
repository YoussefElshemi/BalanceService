using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;

namespace Core.Services;

public class AccountService(
    IAccountRepository accountRepository,
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
            AccountType = createAccountRequest.AccountType,
            AccountStatus = AccountStatus.PendingActivation,
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
        var account = await accountRepository.GetByIdAsync(accountId, cancellationToken) 
                      ?? throw new NotFoundException();

        return account;
    }

    public Task<bool> ExistsAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        return accountRepository.ExistsAsync(accountId, cancellationToken);
    }

    public async Task<Account> UpdateAsync(AccountId accountId, UpdateAccountRequest updateAccountRequest, CancellationToken cancellationToken)
    {
        if (!await accountRepository.ExistsAsync(accountId, cancellationToken))
        {
            throw new NotFoundException();
        }

        if (accountId == updateAccountRequest.ParentAccountId)
        {
            throw new UnprocessableRequestException($"{nameof(AccountId)} must not match {nameof(UpdateAccountRequest.ParentAccountId)}");
        }

        var account = await accountRepository.UpdateAsync(accountId, updateAccountRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return account;
    }

    public async Task DeleteAsync(AccountId accountId, Username deletedBy, CancellationToken cancellationToken)
    {
        await accountRulesService.ThrowIfNotAllowedAsync(accountId, AccountOperationType.DeleteAccount, cancellationToken);

        await accountRepository.DeleteAsync(accountId, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<AccountBalance> GetBalancesByIdAsync(AccountId accountId, CancellationToken cancellationToken)
    {
        var accountBalances = await accountRepository.GetBalancesByIdAsync(accountId, cancellationToken)
                              ?? throw new NotFoundException();

        return accountBalances;
    }

    public async Task ActivateAsync(AccountId accountId, Username activatedBy, CancellationToken cancellationToken)
    {
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
        var account = await GetByIdAsync(accountId, cancellationToken);

        accountRulesService.ThrowIfNotAllowed(account.AccountStatus, AccountOperationType.CloseAccount);

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

    private async Task UpdateStatusAsync(UpdateAccountStatusRequest updateAccountStatusRequest, CancellationToken cancellationToken)
    {
        await accountRepository.UpdateStatusAsync(updateAccountStatusRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}