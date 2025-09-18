using System.Text.Json;
using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class QueryInterestProductAccountLinksRequestDtoMapper
{
    public static QueryInterestProductAccountLinksRequest ToModel(this QueryInterestProductAccountLinksRequestDto queryInterestProductAccountLinksRequestDto)
    {
        return new QueryInterestProductAccountLinksRequest
        {
            PageSize = new PageSize(queryInterestProductAccountLinksRequestDto.PageSize),
            PageNumber = new PageNumber(queryInterestProductAccountLinksRequestDto.PageNumber),
            IsActive = queryInterestProductAccountLinksRequestDto.IsActive
        };
    }
}