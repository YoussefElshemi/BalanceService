namespace Infrastructure.Entities.History;

public record ChangeEventEntity
{
    public required Guid EntityId { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string Field { get; init; } = null!;
    public required string? OldValue { get; init; }
    public required string? NewValue { get; init; }
}