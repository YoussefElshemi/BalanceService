namespace Infrastructure.Entities;

public record HoldEntity : DeletableBaseEntity
{
    public required Guid HoldId { get; init; }
    public required Guid AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required Guid IdempotencyKey { get; init; }
    public required int HoldTypeId { get; set; }
    public required int HoldStatusId { get; set; }
    public required int HoldSourceId { get; init; }
    public required Guid? SettledTransactionId  { get; set; }
    public required DateTimeOffset? ExpiresAt { get; set; }
    public required string? Description { get; set; }
    public required string? Reference { get; set; }

    public AccountEntity AccountEntity { get; init; } = null!;
    public TransactionEntity? SettledTransactionEntity { get; init; }
    public HoldTypeEntity HoldTypeEntity { get; init; } = null!;
    public HoldStatusEntity HoldStatusEntity { get; init; } = null!;
    public HoldSourceEntity HoldSourceEntity { get; init; } = null!;
}
