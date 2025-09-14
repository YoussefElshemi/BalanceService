using Core.ValueObjects;

namespace Core.Models;

public record ChangeEvent
{
    public required EntityId EntityId { get; init; }
    public required Timestamp Timestamp { get; init; }
    public required ChangeEventField Field { get; init; }
    public required ChangeEventValue? OldValue { get; init; }
    public required ChangeEventValue? NewValue { get; init; }
}
