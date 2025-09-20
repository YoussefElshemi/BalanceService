using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Holds;

public record GetHoldHistoryRequestDto : GetHistoryRequestDto
{
    [FromRoute]
    public required Guid HoldId { get; init; }
}