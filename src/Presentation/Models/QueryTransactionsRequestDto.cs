using System.ComponentModel;
using Core.Enums;

namespace Presentation.Models;

public record QueryTransactionsRequestDto
{
    [DefaultValue(20)]
    public required int PageSize { get; init; } = 20;
    [DefaultValue(1)]
    public required int PageNumber { get; init; } = 1;
    public Guid? AccountId { get; init; }
    public CurrencyCode? CurrencyCode { get; init; }
    public decimal? Amount { get; init; }
    public DateOnly? FromPostedDate { get; init; }
    public DateOnly? ToPostedDate { get; init; }
    public TransactionDirection? Direction { get; init; }
    public TransactionType? Type { get; init; }
    public TransactionStatus? Status { get; init; }
    public TransactionSource? Source { get; init; }
    public string? Description { get; init; }
    public string? Reference { get; init; }
}
