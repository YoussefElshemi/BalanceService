namespace Presentation.Models;

public record PagedResultsDto<T>
{
    public required List<T> Data { get; init; }
    public required PagedMetadataDto MetaData { get; init; }
}