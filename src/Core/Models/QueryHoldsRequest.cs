using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record QueryHoldsRequest
{
    public required PageSize PageSize { get; init; }
    public required PageNumber PageNumber { get; init; }
    public required AccountId? AccountId { get; init; }
    public required CurrencyCode? CurrencyCode { get; init; }
    public required HoldAmount? Amount { get; init; }
    public required TransactionId? SettledTransactionId { get; init; }
    public required ExpiresAt? ExpiresAt { get; init; }
    public required HoldType? Type { get; init; }
    public required HoldStatus? Status { get; init; }
    public required HoldSource? Source { get; init; }
    public required HoldDescription? Description { get; init; }
    public required HoldReference? Reference { get; init; }
}