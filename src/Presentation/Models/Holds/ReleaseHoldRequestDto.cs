using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Holds;

public record ReleaseHoldRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid HoldId { get; init; }
}