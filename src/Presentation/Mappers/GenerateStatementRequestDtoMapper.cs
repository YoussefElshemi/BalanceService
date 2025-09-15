using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class GenerateStatementRequestDtoMapper
{
    public static GenerateStatementRequest ToModel(this GenerateStatementRequestDto generateStatementRequestDto)
    {
        return new GenerateStatementRequest
        {
            AccountId =  new AccountId(generateStatementRequestDto.AccountId),
            DateRange = new Range<DateOnly>
            {
                From = generateStatementRequestDto.FromDate,
                To = generateStatementRequestDto.ToDate
            },
            Direction = generateStatementRequestDto.Direction
        };
    }
}