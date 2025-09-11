namespace Core.Models;

public record PagedResults<T>
{
    public required List<T> Data { get; init; }
    public required PagedMetadata MetaData { get; init; }
}