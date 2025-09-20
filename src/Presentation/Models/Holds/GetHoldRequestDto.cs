using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Holds;

public record GetHoldRequestDto : BaseReadRequestDto
{
    [FromRoute]
    public required Guid HoldId { get; init; }
}