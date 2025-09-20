using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Transactions;

public record DeleteTransactionRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid TransactionId { get; init; }
}