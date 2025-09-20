using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.InterestProducts;

public record UpdateInterestProductRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid InterestProductId { get; init; }
    
    [FromBody]
    public required string Name { get; init; }
    
    [FromBody]
    public required decimal AnnualInterestRate { get; init; }
    
    [FromBody]
    public required InterestPayoutFrequency InterestPayoutFrequency { get; init; }
    
    [FromBody]
    public required int AccrualBasis { get; init; }
}