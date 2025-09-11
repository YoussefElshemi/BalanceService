namespace Infrastructure.Entities;

public record TransactionDirectionEntity
{
    public required int TransactionDirectionId { get; init; }
    public required string Name { get; init; }
    public ICollection<TransactionEntity> TransactionEntities { get; init; } = null!;
}
