using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Transactions;

public record UpdateTransactionRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid TransactionId { get; init; }

    [FromBody]
    public required TransactionType Type { get; init; }
    
    [FromBody]
    public required TransactionSource Source { get; init; }
    
    [FromBody]
    public string? Description { get; init; }
    
    [FromBody]
    public string? Reference { get; init; }
}
