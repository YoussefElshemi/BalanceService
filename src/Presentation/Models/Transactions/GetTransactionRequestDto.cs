using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Transactions;

public record GetTransactionRequestDto : BaseReadRequestDto
{
    [FromRoute]
    public required Guid TransactionId { get; init; }
}