using Core.Models;
using Core.ValueObjects;
using Presentation.Models.InterestProducts;

namespace Presentation.Mappers.InterestProducts;

public static class QueryInterestProductsRequestDtoMapper
{
    public static QueryInterestProductsRequest ToModel(this QueryInterestProductsRequestDto queryInterestProductsRequestDto)
    {
        return new QueryInterestProductsRequest
        {
            PageSize = new PageSize(queryInterestProductsRequestDto.PageSize),
            PageNumber = new PageNumber(queryInterestProductsRequestDto.PageNumber),
            Name = !string.IsNullOrWhiteSpace(queryInterestProductsRequestDto.Name)
                ? new InterestProductName(queryInterestProductsRequestDto.Name)
                : null,
            InterestPayoutFrequency = queryInterestProductsRequestDto.InterestPayoutFrequency
        };
    }
}