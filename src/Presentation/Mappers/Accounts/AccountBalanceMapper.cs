using Core.Models;
using Presentation.Models.Accounts;

namespace Presentation.Mappers.Accounts;

public static class AccountBalanceMapper
{
    public static AccountBalanceDto ToDto(this AccountBalance account)
    {
        return new AccountBalanceDto
        {
            AccountId = account.AccountId,
            AvailableBalance = account.AvailableBalance,
            LedgerBalance = account.LedgerBalance,
            PendingBalance = account.PendingBalance,
            HoldBalance = account.HoldBalance
        };
    }
}