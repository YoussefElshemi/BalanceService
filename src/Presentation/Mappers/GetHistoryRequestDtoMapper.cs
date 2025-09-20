using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class GetHistoryRequestDtoMapper
{
    public static GetHistoryRequest ToModel(this GetHistoryRequestDto queryAccountsRequestDto, Guid entityId)
    {
        return new GetHistoryRequest
        {
            PageSize = new PageSize(queryAccountsRequestDto.PageSize),
            PageNumber = new PageNumber(queryAccountsRequestDto.PageNumber),
            EntityId = entityId,
            IgnoreInsert = queryAccountsRequestDto.IgnoreInsert
        };
    }
}