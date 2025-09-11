using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record QueryTransactionsRequest
{
    public required PageSize PageSize { get; init; }
    public required PageNumber PageNumber { get; init; }
    public required AccountId? AccountId { get; init; }
    public required CurrencyCode? CurrencyCode { get; init; }
    public required TransactionAmount? Amount { get; init; }
    public required TransactionDirection? Direction { get; init; }
    public required PostedDate? PostedDate { get; init; }
    public required TransactionType? Type { get; init; }
    public required TransactionStatus? Status { get; init; }
    public required TransactionSource? Source { get; init; }
    public required TransactionDescription? Description { get; init; }
    public required TransactionReference? Reference { get; init; }
}