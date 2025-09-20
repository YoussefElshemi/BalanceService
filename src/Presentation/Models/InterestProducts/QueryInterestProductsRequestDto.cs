using System.ComponentModel;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.InterestProducts;

public record QueryInterestProductsRequestDto
{
    [DefaultValue(20)]
    [FromQuery]
    public required int PageSize { get; init; } = 20;

    [DefaultValue(1)]
    [FromQuery]
    public required int PageNumber { get; init; } = 1;
    
    [FromQuery]
    public string? Name { get; init; }
    
    [FromQuery]
    public InterestPayoutFrequency? InterestPayoutFrequency { get; init; }
}
