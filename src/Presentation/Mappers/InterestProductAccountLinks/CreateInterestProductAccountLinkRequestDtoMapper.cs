using Core.Models;
using Core.ValueObjects;
using Presentation.Models.InterestProductAccountLinks;

namespace Presentation.Mappers.InterestProductAccountLinks;

public static class CreateInterestProductAccountLinkRequestDtoMapper
{
    public static CreateInterestProductAccountLinkRequest ToModel(
        this CreateInterestProductAccountLinkRequestDto createInterestProductAccountLinkRequestDto)
    {
        return new CreateInterestProductAccountLinkRequest
        {
            AccountId = new AccountId(createInterestProductAccountLinkRequestDto.AccountId),
            InterestProductId = new InterestProductId(createInterestProductAccountLinkRequestDto.InterestProductId),
            ExpiresAt = createInterestProductAccountLinkRequestDto.ExpiresAt.HasValue
                ? new  ExpiresAt(createInterestProductAccountLinkRequestDto.ExpiresAt.Value)
                : null,
            CreatedBy = new Username(createInterestProductAccountLinkRequestDto.Username)
        };
    }
}