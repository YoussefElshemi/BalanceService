namespace Presentation.Models.Accounts;

public record AccountBalanceDto
{
    public required Guid AccountId { get; init; }
    public required decimal AvailableBalance  { get; init; }
    public required decimal LedgerBalance  { get; init; }
    public required decimal PendingDebitBalance  { get; init; }
    public required decimal PendingCreditBalance  { get; init; }
    public required decimal HoldBalance  { get; init; }
}