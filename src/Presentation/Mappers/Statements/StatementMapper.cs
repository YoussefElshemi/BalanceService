using Core.Models;
using Presentation.Models.Statements;

namespace Presentation.Mappers.Statements;

public static class StatementMapper
{
    public static StatementDto ToDto(this Statement statement)
    {
        return new StatementDto
        {
            AccountId = statement.AccountId,
            DateRange = statement.DateRange.ToDto(),
            OpeningBalance = statement.OpeningBalance,
            ClosingBalance = statement.ClosingBalance,
            StatementEntries = statement.StatementEntries.ToDto(x => x.ToDto())
        };
    }
}