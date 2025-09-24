namespace Infrastructure.Entities;

public interface IHistoryEntity<TModel>
{
    public static abstract string GetIdColumn();
    public static abstract string[] GetColumns();
    public DateTimeOffset Timestamp { get; init; }
    public int ProcessingStatusId { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public TModel ToModel();
}