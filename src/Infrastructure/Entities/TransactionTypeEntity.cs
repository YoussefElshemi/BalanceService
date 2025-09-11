namespace Infrastructure.Entities;

public record TransactionTypeEntity
{
    public required int TransactionTypeId { get; init; }
    public required string Name { get; init; }
    public ICollection<TransactionEntity> TransactionEntities { get; init; } = null!;
}
