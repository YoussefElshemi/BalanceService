using Core.Models;
using Core.ValueObjects;
using Presentation.Models.Statements;

namespace Presentation.Mappers.Statements;

public static class GetStatementRequestDtoMapper
{
    public static GetStatementRequest ToModel(this GetStatementRequestDto getStatementRequestDto)
    {
        return new GetStatementRequest
        {
            PageSize = new PageSize(getStatementRequestDto.PageSize),
            PageNumber = new PageNumber(getStatementRequestDto.PageNumber),
            AccountId =  new AccountId(getStatementRequestDto.AccountId),
            DateRange = new Range<DateOnly>
            {
                From = getStatementRequestDto.FromDate,
                To = getStatementRequestDto.ToDate
            },
            Direction = getStatementRequestDto.Direction
        };
    }
}