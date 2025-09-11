using System.Text.Json;
using Core.Enums;

namespace Presentation.Models;

public record UpdateAccountRequestDto
{
    public required string AccountName { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public decimal? MinimumRequiredBalance { get; init; }
    public required AccountType AccountType { get; init; }
    public JsonDocument? Metadata { get; init; }
    public Guid? ParentAccountId { get; init; }
}
