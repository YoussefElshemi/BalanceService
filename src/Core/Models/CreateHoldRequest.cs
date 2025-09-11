using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record CreateHoldRequest
{

    public required AccountId AccountId { get; init; }
    public required HoldAmount Amount { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public required IdempotencyKey IdempotencyKey { get; init; }
    public required HoldStatus Status { get; init; }
    public required HoldType Type { get; init; }
    public required HoldSource Source { get; init; }
    public required ExpiresAt? ExpiresAt { get; init; }
    public required HoldDescription? Description { get; init; }
    public required HoldReference? Reference { get; init; }
    public required Username CreatedBy { get; init; }
}