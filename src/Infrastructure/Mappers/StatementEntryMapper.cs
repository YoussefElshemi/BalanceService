using Core.Enums;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;

namespace Infrastructure.Mappers;

public static class StatementEntryMapper
{
    public static StatementEntry ToModel(this StatementEntryEntity statementEntryEntity)
    {
        return new StatementEntry
        {
            StatementEntryId = new StatementEntryId(statementEntryEntity.StatementEntryId),
            Date = new StatementDate(DateOnly.FromDateTime(statementEntryEntity.ActionedAt.UtcDateTime)),
            AvailableBalance = new AvailableBalance(statementEntryEntity.AvailableBalance),
            Amount = new StatementAmount(statementEntryEntity.Amount),
            CurrencyCode = Enum.Parse<CurrencyCode>(statementEntryEntity.CurrencyCode),
            Type = (StatementType)statementEntryEntity.StatementTypeId,
            Direction = (StatementDirection)statementEntryEntity.StatementDirectionId,
            Status = (StatementStatus)statementEntryEntity.StatementStatusId,
            Description = !string.IsNullOrWhiteSpace(statementEntryEntity.Description)
                ? new StatementDescription(statementEntryEntity.Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(statementEntryEntity.Reference)
                ? new StatementReference(statementEntryEntity.Reference)
                : null,
        };
    }
}