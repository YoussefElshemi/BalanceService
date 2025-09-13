using Core.Enums;

namespace Core.Dtos;

public record HistoryDto<TDto>
{
    public required Guid HistoryId { get; init; }
    public required HistoryType Type { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required TDto Data { get; init; }
}