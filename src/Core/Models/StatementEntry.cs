using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record StatementEntry
{
    public required StatementEntryId StatementEntryId { get; init; }
    public required StatementDate Date { get; init; }
    public required AvailableBalance AvailableBalance { get; init; }
    public required StatementAmount Amount { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public required StatementType Type { get; init; }
    public required StatementDirection Direction { get; init; }
    public required StatementDescription? Description { get; init; }
    public required StatementReference? Reference { get; init; }
}
