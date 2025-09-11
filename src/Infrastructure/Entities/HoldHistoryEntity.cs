namespace Infrastructure.Entities;

public record HoldHistoryEntity
{
    public required Guid HoldHistoryId { get; init; }
    public required int HistoryTypeId { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required Guid HoldId { get; init; }
    public required Guid AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required Guid IdempotencyKey { get; init; }
    public required int HoldTypeId { get; init; }
    public required int HoldStatusId { get; init; }
    public required int HoldSourceId { get; init; }
    public required Guid? SettledTransactionId  { get; init; }
    public required DateTimeOffset? ExpiresAt { get; init; }
    public required string? Description { get; init; }
    public required string? Reference { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTimeOffset? DeletedAt { get; init; }
    public required string? DeletedBy { get; init; }

    public AccountEntity AccountEntity { get; init; } = null!;
    public TransactionEntity? SettledTransactionEntity { get; init; }
    public HoldTypeEntity HoldTypeEntity { get; init; } = null!;
    public HoldStatusEntity HoldStatusEntity { get; init; } = null!;
    public HoldSourceEntity HoldSourceEntity { get; init; } = null!;
    public HistoryTypeEntity HistoryTypeEntity { get; init; } = null!;

}
