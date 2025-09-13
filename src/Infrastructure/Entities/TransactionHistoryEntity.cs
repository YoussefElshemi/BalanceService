namespace Infrastructure.Entities;

public record TransactionHistoryEntity
{
    public required Guid TransactionHistoryId { get; init; }
    public required int HistoryTypeId { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required Guid TransactionId { get; init; }
    public required Guid AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required int TransactionDirectionId { get; init; }
    public required DateTimeOffset? PostedAt  { get; init; }
    public required Guid IdempotencyKey { get; init; }
    public required int TransactionTypeId { get; init; }
    public required int TransactionStatusId { get; init; }
    public required int TransactionSourceId { get; init; }
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
    public TransactionDirectionEntity TransactionDirectionEntity { get; init; } = null!;
    public TransactionTypeEntity TransactionTypeEntity { get; init; } = null!;
    public TransactionStatusEntity TransactionStatusEntity { get; init; } = null!;
    public TransactionSourceEntity TransactionSourceEntity { get; init; } = null!;
    public HistoryTypeEntity HistoryTypeEntity { get; init; } = null!;

}
