using System.ComponentModel;
using Core.Enums;

namespace Presentation.Models;

public record QueryInterestProductsRequestDto
{
    [DefaultValue(20)]
    public required int PageSize { get; init; } = 20;
    [DefaultValue(1)]
    public required int PageNumber { get; init; } = 1;
    public string? Name { get; init; }
    public InterestPayoutFrequency? InterestPayoutFrequency { get; init; }
}
