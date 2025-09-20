using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Holds;

public record CreateHoldRequestDto : BaseWriteRequestDto
{
    [FromBody]
    public required Guid AccountId { get; init; }
    
    [FromBody]
    public required decimal Amount { get; init; }
    
    [FromBody]
    public required CurrencyCode CurrencyCode { get; init; }
    
    [FromBody]
    public required Guid IdempotencyKey { get; init; }
    
    [FromBody]
    public required HoldType Type { get; init; }
    
    [FromBody]
    public DateTimeOffset? ExpiresAt { get; init; }
    
    [FromBody]
    public string? Description { get; init; }
    
    [FromBody]
    public string? Reference { get; init; }
}