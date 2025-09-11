using Core.Models;
using Presentation.Models;

namespace Presentation.Mappers;

public static class AccountMapper
{
    public static AccountDto ToDto(this Account account)
    {
        return new AccountDto
        {
            AccountId = account.AccountId,
            AccountName = account.AccountName,
            CurrencyCode = account.CurrencyCode,
            AvailableBalance = account.AvailableBalance,
            LedgerBalance = account.LedgerBalance,
            PendingBalance = account.PendingBalance,
            HoldBalance = account.HoldBalance,
            MinimumRequiredBalance = account.MinimumRequiredBalance,
            AccountType = account.AccountType,
            AccountStatus = account.AccountStatus,
            Metadata = account.Metadata,
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
}