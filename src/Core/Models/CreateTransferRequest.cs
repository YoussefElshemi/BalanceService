using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record CreateTransferRequest
{
    public required AccountId DebitAccountId { get; init; }
    public required AccountId CreditAccountId { get; init; }
    public required TransferAmount Amount { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public required IdempotencyKey DebitIdempotencyKey { get; init; }
    public required IdempotencyKey CreditIdempotencyKey { get; init; }
    public required TransferSource Source { get; init; }
    public required TransferDescription? Description { get; init; }
    public required TransferReference? Reference { get; init; }
    public required Username CreatedBy { get; init; }
}
