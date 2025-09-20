using Core.Models;
using Core.ValueObjects;
using Presentation.Models.Accounts;

namespace Presentation.Mappers.Accounts;

public static class UpdateAccountRequestDtoMapper
{
    public static UpdateAccountRequest ToModel(this UpdateAccountRequestDto updateAccountRequestDto)
    {
        return new UpdateAccountRequest
        {
            AccountId = new AccountId(updateAccountRequestDto.AccountId),
            AccountName = new AccountName(updateAccountRequestDto.AccountName),
            CurrencyCode = updateAccountRequestDto.CurrencyCode,
            MinimumRequiredBalance = updateAccountRequestDto.MinimumRequiredBalance.HasValue
                ? new MinimumRequiredBalance(updateAccountRequestDto.MinimumRequiredBalance.Value)
                : new MinimumRequiredBalance(0),
            AccountType = updateAccountRequestDto.AccountType,
            Metadata = updateAccountRequestDto.Metadata,
            ParentAccountId = updateAccountRequestDto.ParentAccountId.HasValue
                ? new AccountId(updateAccountRequestDto.ParentAccountId.Value)
                : null,
            UpdatedBy = new Username(updateAccountRequestDto.Username)
        };
    }
}