using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.InterestProducts;

public record DeleteInterestProductRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid InterestProductId { get; init; }
}