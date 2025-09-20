using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Holds;

public record SettleHoldRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid HoldId { get; init; }
}