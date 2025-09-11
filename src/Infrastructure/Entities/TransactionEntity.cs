namespace Infrastructure.Entities;

public record TransactionEntity : DeletableBaseEntity
{
    public required Guid TransactionId { get; init; }
    public required Guid AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required int TransactionDirectionId { get; init; }
    public required DateOnly? PostedDate  { get; set; }
    public required Guid IdempotencyKey { get; init; }
    public required int TransactionTypeId { get; set; }
    public required int TransactionStatusId { get; set; }
    public required int TransactionSourceId { get; set; }
    public required string? Description { get; set; }
    public required string? Reference { get; set; }

    public AccountEntity AccountEntity { get; init; } = null!;
    public TransactionDirectionEntity TransactionDirectionEntity { get; init; } = null!;
    public TransactionTypeEntity TransactionTypeEntity { get; init; } = null!;
    public TransactionStatusEntity TransactionStatusEntity { get; init; } = null!;
    public TransactionSourceEntity TransactionSourceEntity { get; init; } = null!;
}
