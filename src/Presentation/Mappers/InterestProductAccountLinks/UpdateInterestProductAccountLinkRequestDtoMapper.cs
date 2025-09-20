using Core.Models;
using Core.ValueObjects;
using Presentation.Models.InterestProductAccountLinks;

namespace Presentation.Mappers.InterestProductAccountLinks;

public static class UpdateInterestProductAccountLinkRequestDtoMapper
{
    public static UpdateInterestProductAccountLinkRequest ToModel(
        this UpdateInterestProductAccountLinkRequestDto updateInterestProductAccountLinkRequestDto)
    {
        return new UpdateInterestProductAccountLinkRequest
        {
            AccountId = new AccountId(updateInterestProductAccountLinkRequestDto.AccountId),
            InterestProductId = new InterestProductId(updateInterestProductAccountLinkRequestDto.InterestProductId),
            UpdatedBy = new Username(updateInterestProductAccountLinkRequestDto.Username)
        };
    }
}