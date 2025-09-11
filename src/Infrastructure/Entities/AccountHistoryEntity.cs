namespace Infrastructure.Entities;

public record AccountHistoryEntity
{
    public required Guid AccountHistoryId { get; init; }
    public required int HistoryTypeId { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required Guid AccountId { get; init; }
    public required string AccountName { get; init; }
    public required string CurrencyCode { get; init; }
    public required decimal LedgerBalance { get; init; }
    public required decimal AvailableBalance { get; init; }
    public required decimal PendingBalance { get; init; }
    public required decimal HoldBalance { get; init; }
    public required decimal MinimumRequiredBalance { get; init; }
    public required int AccountTypeId { get; init; }
    public required int AccountStatusId { get; init; }
    public required string? Metadata { get; init; }
    public required Guid? ParentAccountId { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTimeOffset? DeletedAt { get; init; }
    public required string? DeletedBy { get; init; }

    public AccountEntity? ParentAccountEntity { get; init; }
    public AccountTypeEntity AccountTypeEntity { get; init; } = null!;
    public AccountStatusEntity AccountStatusEntity { get; init; } = null!;
    public HistoryTypeEntity HistoryTypeEntity { get; init; } = null!;
}