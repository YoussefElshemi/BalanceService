using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.InterestProductAccountLinks;

public record GetInterestProductAccountLinkRequestDto : BaseReadRequestDto
{
    [FromRoute]
    public required Guid InterestProductId { get; init; }

    [FromRoute]
    public required Guid AccountId { get; init; }
}