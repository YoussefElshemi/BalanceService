using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class UpdateAccountRequestDtoMapper
{
    public static UpdateAccountRequest ToModel(this UpdateAccountRequestDto updateAccountRequestDto, string username)
    {
        return new UpdateAccountRequest
        {
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
            UpdatedBy = new Username(username)
        };
    }
}