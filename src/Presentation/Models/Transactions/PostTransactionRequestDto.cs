using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Transactions;

public record PostTransactionRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid TransactionId { get; init; }
}