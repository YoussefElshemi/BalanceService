namespace Presentation.Models;

public record RangeDto<T>
{
    public required T From { get; init; }
    public required T To { get; init; }
}