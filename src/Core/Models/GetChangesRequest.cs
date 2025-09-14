using Core.ValueObjects;

namespace Core.Models;

public record GetChangesRequest
{
    public required PageSize PageSize { get; init; }
    public required PageNumber PageNumber { get; init; }
    public required Guid EntityId { get; init; }
}
