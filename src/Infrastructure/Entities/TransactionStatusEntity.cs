namespace Infrastructure.Entities;

public record TransactionStatusEntity
{
    public required int TransactionStatusId { get; init; }
    public required string Name { get; init; }
    public ICollection<TransactionEntity> TransactionEntities { get; init; } = null!;
}
