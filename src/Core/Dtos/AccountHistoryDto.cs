using System.Text.Json;
using Core.Enums;

namespace Core.Dtos;

public record AccountHistoryDto
{
    public required Guid AccountId { get; init; }
    public required string AccountName { get; init; }
    public required CurrencyCode CurrencyCode  { get; init; }
    public required decimal AvailableBalance  { get; init; }
    public required decimal LedgerBalance  { get; init; }
    public required decimal PendingDebitBalance  { get; init; }
    public required decimal PendingCreditBalance  { get; init; }
    public required decimal HoldBalance  { get; init; }
    public required decimal MinimumRequiredBalance  { get; init; }
    public required AccountType AccountType { get; init; }
    public required AccountStatus AccountStatus { get; init; }
    public required JsonDocument? Metadata { get; init; }
    public required Guid? ParentAccountId { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTimeOffset? DeletedAt { get; init; }
    public required string? DeletedBy { get; init; }
}