namespace Infrastructure.Entities.History;

public interface IHistoryEntity<TModel>
{
    public Guid GetPrimaryKey();
    public DateTimeOffset Timestamp { get; init; }
    public int ProcessingStatusId { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public TModel ToModel();
}