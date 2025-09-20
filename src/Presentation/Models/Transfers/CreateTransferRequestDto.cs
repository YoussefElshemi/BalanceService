using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Transfers;

public record CreateTransferRequestDto : BaseWriteRequestDto
{
    [FromBody]
    public required Guid DebitAccountId { get; init; }
    
    [FromBody]
    public required Guid CreditAccountId { get; init; }
    
    [FromBody]
    public required decimal Amount { get; init; }
    
    [FromBody]
    public required CurrencyCode CurrencyCode { get; init; }
    
    [FromBody]
    public required Guid DebitIdempotencyKey { get; init; }
    
    [FromBody]
    public required Guid CreditIdempotencyKey { get; init; }
    
    [FromBody]
    public string? Description { get; init; }
    
    [FromBody]
    public string? Reference { get; init; }
}
