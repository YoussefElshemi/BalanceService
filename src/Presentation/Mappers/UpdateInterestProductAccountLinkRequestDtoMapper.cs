using Core.Enums;
using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class UpdateInterestProductAccountLinkRequestDtoMapper
{
    public static UpdateInterestProductAccountLinkRequest ToModel(
        this UpdateInterestProductAccountLinkRequestDto updateInterestProductAccountLinkRequestDto,
        Guid accountId,
        Guid interestProductId,
        string username)
    {
        return new UpdateInterestProductAccountLinkRequest
        {
            AccountId = new AccountId(accountId),
            InterestProductId = new InterestProductId(interestProductId),
            UpdatedBy = new Username(username)
        };
    }
}