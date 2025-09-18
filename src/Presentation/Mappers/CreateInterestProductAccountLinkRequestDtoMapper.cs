using Core.Enums;
using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class CreateInterestProductAccountLinkRequestDtoMapper
{
    public static CreateInterestProductAccountLinkRequest ToModel(
        this CreateInterestProductAccountLinkRequestDto createInterestProductAccountLinkRequestDto,
        Guid accountId,
        string username)
    {
        return new CreateInterestProductAccountLinkRequest
        {
            AccountId = new AccountId(accountId),
            InterestProductId = new InterestProductId(createInterestProductAccountLinkRequestDto.InterestProductId),
            CreatedBy = new Username(username)
        };
    }
}