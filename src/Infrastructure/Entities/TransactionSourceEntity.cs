namespace Infrastructure.Entities;

public record TransactionSourceEntity
{
    public required int TransactionSourceId { get; init; }
    public required string Name { get; init; }
    public ICollection<TransactionEntity> TransactionEntities { get; init; } = null!;
}
