using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record CreateTransactionRequest
{
    public required AccountId AccountId { get; init; }
    public required TransactionAmount Amount { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public required TransactionDirection Direction { get; init; }
    public required IdempotencyKey IdempotencyKey { get; init; }
    public required TransactionType Type { get; init; }
    public required TransactionSource Source { get; init; }
    public required TransactionStatus Status { get; init; }
    public required PostedDate? PostedDate { get; init; }
    public required TransactionDescription? Description { get; init; }
    public required TransactionReference? Reference { get; init; }
    public required Username CreatedBy { get; init; }
}
