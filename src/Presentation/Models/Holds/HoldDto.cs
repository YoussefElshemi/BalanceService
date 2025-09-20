using Core.Enums;

namespace Presentation.Models.Holds;

public record HoldDto
{
    public required Guid HoldId { get; init; }
    public required Guid AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public required Guid IdempotencyKey { get; init; }
    public required HoldType Type { get; init; }
    public required HoldStatus Status { get; init; }
    public required HoldSource Source { get; init; }
    public required Guid? SettledTransactionId { get; init; }
    public required DateTimeOffset? ExpiresAt { get; init; }
    public required string? Description { get; init; }
    public required string? Reference { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTimeOffset? DeletedAt { get; init; }
    public required string? DeletedBy { get; init; }
}