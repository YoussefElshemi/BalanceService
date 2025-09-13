namespace Presentation.Models;

public record StatementDto
{
    public required Guid AccountId { get; init; }
    public required RangeDto<DateOnly> DateRange { get; init; }
    public required decimal OpeningBalance { get; init; }
    public required decimal ClosingBalance { get; init; }
    public required PagedResultsDto<StatementEntryDto> StatementEntries { get; init; }
}