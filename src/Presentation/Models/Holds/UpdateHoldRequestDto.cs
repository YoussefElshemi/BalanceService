using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Holds;

public record UpdateHoldRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid HoldId { get; init; }
    
    [FromBody]
    public required HoldType Type { get; init; }
    
    [FromBody]
    public DateTimeOffset? ExpiresAt { get; init; }
    
    [FromBody]
    public string? Description { get; init; }
    
    [FromBody]
    public string? Reference { get; init; }
}