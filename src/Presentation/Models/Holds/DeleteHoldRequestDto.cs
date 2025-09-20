using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Holds;

public record DeleteHoldRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid HoldId { get; init; }
}