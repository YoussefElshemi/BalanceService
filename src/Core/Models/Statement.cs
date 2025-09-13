using Core.ValueObjects;

namespace Core.Models;

public record Statement
{
    public required AccountId AccountId { get; init; }
    public required Range<DateOnly> DateRange { get; init; }
    public required AvailableBalance OpeningBalance { get; init; }
    public required AvailableBalance ClosingBalance { get; init; }
    public required PagedResults<StatementEntry> StatementEntries { get; init;}
}
