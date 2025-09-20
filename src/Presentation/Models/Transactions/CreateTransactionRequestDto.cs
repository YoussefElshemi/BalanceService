using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Transactions;

public record CreateTransactionRequestDto : BaseWriteRequestDto
{
    [FromBody]
    public required Guid AccountId { get; init; }
    
    [FromBody]
    public required decimal Amount { get; init; }
    
    [FromBody]
    public required CurrencyCode CurrencyCode { get; init; }
    
    [FromBody]
    public required TransactionDirection Direction { get; init; }
    
    [FromBody]
    public required Guid IdempotencyKey { get; init; }
    
    [FromBody]
    public required TransactionType Type { get; init; }
    
    [FromBody]
    public string? Description { get; init; }
    
    [FromBody]
    public string? Reference { get; init; }
}
