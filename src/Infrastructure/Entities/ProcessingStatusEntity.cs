using Infrastructure.Entities.History;

namespace Infrastructure.Entities;

public record ProcessingStatusEntity
{
    public required int ProcessingStatusId { get; init; }
    public required string Name { get; init; }
    public ICollection<AccountHistoryEntity> AccountHistoryEntities { get; init; } = null!;
    public ICollection<TransactionHistoryEntity> TransactionHistoryEntities { get; init; } = null!;
    public ICollection<HoldHistoryEntity> HoldHistoryEntities { get; init; } = null!;
}
