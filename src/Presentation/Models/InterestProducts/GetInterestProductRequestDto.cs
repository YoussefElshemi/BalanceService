using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.InterestProducts;

public record GetInterestProductRequestDto : BaseReadRequestDto
{
    [FromRoute]
    public required Guid InterestProductId { get; init; }
}