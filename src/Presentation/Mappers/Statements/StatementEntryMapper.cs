using Core.Models;
using Presentation.Models.Statements;

namespace Presentation.Mappers.Statements;

public static class StatementEntryMapper
{
    public static StatementEntryDto ToDto(this StatementEntry statementEntry)
    {
        return new StatementEntryDto
        {
            StatementEntryId = statementEntry.StatementEntryId,
            Date = statementEntry.Date,
            AvailableBalance = statementEntry.AvailableBalance,
            Amount = statementEntry.Amount,
            CurrencyCode = statementEntry.CurrencyCode,
            Type = statementEntry.Type,
            Direction = statementEntry.Direction,
            Description = statementEntry.Description,
            Reference = statementEntry.Reference
        };
    }
}