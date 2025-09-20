using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Transactions;

public record ReverseTransactionRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid TransactionId { get; init; }
}