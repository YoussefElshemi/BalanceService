using Core.Enums;

namespace Presentation.Models;

public record CreateTransferRequestDto
{
    public required Guid DebitAccountId { get; init; }
    public required Guid CreditAccountId { get; init; }
    public required decimal Amount { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public required Guid DebitIdempotencyKey { get; init; }
    public required Guid CreditIdempotencyKey { get; init; }
    public string? Description { get; init; }
    public string? Reference { get; init; }
}
