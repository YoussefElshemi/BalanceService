using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.InterestProductAccountLinks;

public record CreateInterestProductAccountLinkRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid AccountId { get; init; }

    [FromBody]
    public required Guid InterestProductId { get; init; }
    
    [FromBody]
    public DateTimeOffset? ExpiresAt { get; init; }
}