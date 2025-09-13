namespace Infrastructure.Entities;

public interface IHistoryEntity<TModel>
{
    public Guid GetPrimaryKey();
    public DateTimeOffset Timestamp { get; init; }
    public bool IsProcessed { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public TModel ToModel();
}