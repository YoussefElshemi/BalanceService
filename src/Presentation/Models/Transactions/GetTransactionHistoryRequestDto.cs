using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Transactions;

public record GetTransactionHistoryRequestDto : GetHistoryRequestDto
{
    [FromRoute]
    public required Guid TransactionId { get; init; }
}