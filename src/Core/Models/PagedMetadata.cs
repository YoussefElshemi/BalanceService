namespace Core.Models;

public record PagedMetadata
{
    public required int TotalRecords { get; init; }
    public required int TotalPages { get; init; }
    public required int PageSize { get; init; }
    public required int PageNumber { get; init; }
}
