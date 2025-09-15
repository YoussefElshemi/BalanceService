using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class GeneratePdfStatementRequestDtoMapper
{
    public static GeneratePdfStatementRequest ToModel(this GeneratePdfStatementRequestDto generatePdfStatementRequestDto)
    {
        return new GeneratePdfStatementRequest
        {
            AccountId =  new AccountId(generatePdfStatementRequestDto.AccountId),
            DateRange = new Range<DateOnly>
            {
                From = generatePdfStatementRequestDto.FromDate,
                To = generatePdfStatementRequestDto.ToDate
            },
            Direction = generatePdfStatementRequestDto.Direction
        };
    }
}