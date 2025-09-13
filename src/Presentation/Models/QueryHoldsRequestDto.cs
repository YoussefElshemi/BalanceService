using System.ComponentModel;
using Core.Enums;

namespace Presentation.Models;

public record QueryHoldsRequestDto
{
    [DefaultValue(20)]
    public required int PageSize { get; init; } = 20;
    [DefaultValue(1)]
    public required int PageNumber { get; init; } = 1;
    public Guid? AccountId { get; init; }
    public CurrencyCode? CurrencyCode { get; init; }
    public decimal? Amount { get; init; }
    public Guid? SettledTransactionId { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
    public DateTimeOffset? FromCreatedAt { get; init; }
    public DateTimeOffset? ToCreatedAt { get; init; }
    public HoldType? Type { get; init; }
    public HoldStatus? Status { get; init; }
    public HoldSource? Source { get; init; }
    public string? Description { get; init; }
    public string? Reference { get; init; }
}
