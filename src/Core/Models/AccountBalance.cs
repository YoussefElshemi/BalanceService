using Core.ValueObjects;

namespace Core.Models;

public record AccountBalance
{
    public required AccountId AccountId { get; init; }
    public required AvailableBalance AvailableBalance  { get; init; }
    public required LedgerBalance LedgerBalance { get; init; }
    public required PendingDebitBalance PendingDebitBalance { get; init; }
    public required PendingCreditBalance PendingCreditBalance { get; init; }
    public required HoldBalance HoldBalance { get; init; }
}