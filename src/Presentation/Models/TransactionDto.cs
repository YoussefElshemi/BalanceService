using Core.Enums;

namespace Presentation.Models;

public record TransactionDto
{
    public required Guid TransactionId { get; init; }
    public required Guid AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public required TransactionDirection Direction { get; init; }
    public required DateOnly? PostedDate  { get; init; }
    public required Guid IdempotencyKey { get; init; }
    public required TransactionType Type { get; init; }
    public required TransactionStatus Status { get; init; }
    public required TransactionSource Source { get; init; }
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