using Core.Enums;

namespace Presentation.Models;

public record CreateHoldRequestDto
{

    public required Guid AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public required Guid IdempotencyKey { get; init; }
    public required HoldType Type { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
    public string? Description { get; init; }
    public string? Reference { get; init; }
}