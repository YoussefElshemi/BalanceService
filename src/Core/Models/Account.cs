using System.Text.Json;
using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record Account : DeletableBaseModel
{
    public required AccountId AccountId { get; init; }
    public required AccountName AccountName { get; init; }
    public required CurrencyCode CurrencyCode  { get; init; }
    public required AvailableBalance AvailableBalance  { get; init; }
    public required LedgerBalance LedgerBalance { get; init; }
    public required PendingBalance PendingBalance { get; init; }
    public required HoldBalance HoldBalance { get; init; }
    public required MinimumRequiredBalance MinimumRequiredBalance { get; init; }
    public required AccountType AccountType { get; init; }
    public required AccountStatus AccountStatus { get; init; }
    public required JsonDocument? Metadata { get; init; }
    public required AccountId? ParentAccountId { get; init; }
}