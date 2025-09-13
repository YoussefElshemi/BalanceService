using Core.Enums;

namespace Presentation.Models;

public record StatementEntryDto
{
    public required Guid StatementEntryId { get; init; }
    public required DateOnly Date { get; init; }
    public required decimal AvailableBalance { get; init; }
    public required decimal Amount { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public required StatementType Type { get; init; }
    public required StatementDirection Direction { get; init; }
    public required string? Description { get; init; }
    public required string? Reference { get; init; }
}
