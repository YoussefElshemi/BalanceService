using Core.Models;
using Core.ValueObjects;
using Presentation.Models.Accounts;

namespace Presentation.Mappers.Accounts;

public static class CreateAccountRequestDtoMapper
{
    public static CreateAccountRequest ToModel(this CreateAccountRequestDto createAccountRequestDto)
    {
        return new CreateAccountRequest
        {
            AccountName = new AccountName(createAccountRequestDto.AccountName),
            CurrencyCode = createAccountRequestDto.CurrencyCode,
            AccountType = createAccountRequestDto.AccountType,
            MinimumRequiredBalance = createAccountRequestDto.MinimumRequiredBalance.HasValue
                ? new MinimumRequiredBalance(createAccountRequestDto.MinimumRequiredBalance.Value)
                : new MinimumRequiredBalance(0),
            Metadata = createAccountRequestDto.Metadata,
            ParentAccountId = createAccountRequestDto.ParentAccountId.HasValue
                ? new AccountId(createAccountRequestDto.ParentAccountId.Value)
                : null,
            CreatedBy = new Username(createAccountRequestDto.Username)
        };
    }
}