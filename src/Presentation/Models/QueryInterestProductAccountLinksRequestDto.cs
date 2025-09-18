using System.ComponentModel;
using Core.Enums;

namespace Presentation.Models;

public record QueryInterestProductAccountLinksRequestDto
{
    [DefaultValue(20)]
    public required int PageSize { get; init; } = 20;
    [DefaultValue(1)]
    public required int PageNumber { get; init; } = 1;
    public bool? IsActive { get; init; }
}
