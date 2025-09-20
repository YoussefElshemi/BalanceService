using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.InterestProductAccountLinks;

public record UpdateInterestProductAccountLinkRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid InterestProductId { get; init; }

    [FromRoute]
    public required Guid AccountId { get; init; }

    [FromBody]
    public DateTimeOffset? ExpiresAt { get; set; }
}