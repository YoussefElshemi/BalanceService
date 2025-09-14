using System.Text.Json;
using Core.Enums;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;

namespace Infrastructure.Mappers;

public static class AccountMapper
{
    public static AccountEntity ToEntity(this Account account)
    {
        return new AccountEntity
        {
            AccountId = account.AccountId,
            AccountName = account.AccountName,
            CurrencyCode = account.CurrencyCode.ToString(),
            AvailableBalance = account.AvailableBalance,
            LedgerBalance = account.LedgerBalance,
            PendingBalance = account.PendingBalance,
            HoldBalance = account.HoldBalance,
            MinimumRequiredBalance = account.MinimumRequiredBalance,
            AccountTypeId = (int)account.Type,
            AccountStatusId = (int)account.Status,
            Metadata = account.Metadata?.RootElement.GetRawText(),
            ParentAccountId = account.ParentAccountId,
            CreatedAt = account.CreatedAt,
            CreatedBy = account.CreatedBy,
            UpdatedAt = account.UpdatedAt,
            UpdatedBy = account.UpdatedBy,
            IsDeleted = account.IsDeleted,
            DeletedAt = account.DeletedAt,
            DeletedBy = account.DeletedBy
        };
    }
    
    public static Account ToModel(this AccountEntity accountEntity)
    {
        return new Account
        {
            AccountId = new AccountId(accountEntity.AccountId),
            AccountName = new AccountName(accountEntity.AccountName),
            CurrencyCode = Enum.Parse<CurrencyCode>(accountEntity.CurrencyCode),
            AvailableBalance = new AvailableBalance(accountEntity.AvailableBalance),
            LedgerBalance = new LedgerBalance(accountEntity.LedgerBalance),
            PendingBalance = new PendingBalance(accountEntity.PendingBalance),
            HoldBalance = new HoldBalance(accountEntity.HoldBalance),
            MinimumRequiredBalance = new MinimumRequiredBalance(accountEntity.MinimumRequiredBalance),
            Type = (AccountType)accountEntity.AccountTypeId,
            Status = (AccountStatus)accountEntity.AccountStatusId,
            Metadata = !string.IsNullOrWhiteSpace(accountEntity.Metadata)
                ? JsonDocument.Parse(accountEntity.Metadata)
                : null,
            ParentAccountId = accountEntity.ParentAccountId.HasValue
                ? new AccountId(accountEntity.ParentAccountId.Value)
                : null,
            CreatedAt = new CreatedAt(accountEntity.CreatedAt),
            CreatedBy = new Username(accountEntity.CreatedBy),
            UpdatedAt = new UpdatedAt(accountEntity.UpdatedAt),
            UpdatedBy = new  Username(accountEntity.UpdatedBy),
            IsDeleted = accountEntity.IsDeleted,
            DeletedAt = accountEntity.DeletedAt.HasValue
                ? new DeletedAt(accountEntity.DeletedAt.Value)
                : null,
            DeletedBy = !string.IsNullOrWhiteSpace(accountEntity.DeletedBy)
                ? new Username(accountEntity.DeletedBy)
                : null
        };
    }
}