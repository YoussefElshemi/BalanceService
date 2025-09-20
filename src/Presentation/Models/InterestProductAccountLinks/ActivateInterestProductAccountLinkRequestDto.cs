using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.InterestProductAccountLinks;

public record ActivateInterestProductAccountLinkRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid InterestProductId { get; init; }

    [FromRoute]
    public required Guid AccountId { get; init; }
}