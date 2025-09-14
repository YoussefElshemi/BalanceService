using System.ComponentModel;

namespace Presentation.Models;

public record GetChangesRequestDto
{
    [DefaultValue(20)]
    public required int PageSize { get; init; } = 20;
    [DefaultValue(1)]
    public required int PageNumber { get; init; } = 1;
}