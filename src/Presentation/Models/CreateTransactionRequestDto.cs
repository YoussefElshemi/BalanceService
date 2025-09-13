using Core.Enums;

namespace Presentation.Models;

public record CreateTransactionRequestDto
{
    public required Guid AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public required TransactionDirection Direction { get; init; }
    public required Guid IdempotencyKey { get; init; }
    public required TransactionType Type { get; init; }
    public string? Description { get; init; }
    public string? Reference { get; init; }
}
