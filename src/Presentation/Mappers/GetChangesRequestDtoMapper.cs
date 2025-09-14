using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class GetChangesRequestDtoMapper
{
    public static GetChangesRequest ToModel(this GetChangesRequestDto queryAccountsRequestDto, Guid entityId)
    {
        return new GetChangesRequest
        {
            PageSize = new PageSize(queryAccountsRequestDto.PageSize),
            PageNumber = new PageNumber(queryAccountsRequestDto.PageNumber),
            EntityId = entityId,
            IgnoreInsert = queryAccountsRequestDto.IgnoreInsert
        };
    }
}