namespace Presentation.Models;

public record AccountBalanceDto
{
    public required Guid AccountId { get; init; }
    public required decimal AvailableBalance  { get; init; }
    public required decimal LedgerBalance  { get; init; }
    public required decimal PendingBalance  { get; init; }
    public required decimal HoldBalance  { get; init; }
}