using Core.ValueObjects;

namespace Core.Models;

public record GetHistoryRequest
{
    public required PageSize PageSize { get; init; }
    public required PageNumber PageNumber { get; init; }
    public required Guid EntityId { get; init; }
    public required bool? IgnoreInsert { get; init; }
}
