namespace Infrastructure.Entities;

public record HistoryTypeEntity
{
    public required int HistoryTypeId { get; init; }
    public required string Name { get; init; }
    public ICollection<AccountHistoryEntity> AccountHistoryEntities { get; init; } = null!;
    public ICollection<TransactionHistoryEntity> TransactionHistoryEntities { get; init; } = null!;
}
