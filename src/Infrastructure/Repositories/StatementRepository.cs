using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Entities;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using StatementType = Core.Enums.StatementType;

namespace Infrastructure.Repositories;

public class StatementRepository(ApplicationDbContext dbContext) : IStatementRepository
{
    public Task<int> CountAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken)
    {
        var query = BuildSearchQuery(getStatementRequest);

        return query.CountAsync(cancellationToken);
    }

    public async Task<List<StatementEntry>> QueryAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken)
    {
        var query = BuildSearchQuery(getStatementRequest);

        var entities = await query
            .OrderBy(x => x.ActionedAt)
            .ThenBy(x => x.StatementEntryId)
            .Skip((getStatementRequest.PageNumber - 1) * getStatementRequest.PageSize)
            .Take(getStatementRequest.PageSize)
            .ToListAsync(cancellationToken);

        return entities.Select(x => x.ToModel()).ToList();
    }
    
    public async Task<List<StatementEntry>> QueryAllAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken)
    {
        var query = BuildSearchQuery(getStatementRequest);

        var entities = await query
            .OrderBy(x => x.ActionedAt)
            .ThenBy(x => x.StatementEntryId)
            .ToListAsync(cancellationToken);

        return entities.Select(x => x.ToModel()).ToList();
    }

    private IQueryable<StatementEntryEntity> BuildSearchQuery(GetStatementRequest getStatementRequest)
    {
        var sql = $@"
            SELECT
                e.*,
                SUM(
                    CASE
                        WHEN e.""{nameof(StatementEntryEntity.StatementTypeId)}"" = {(int)StatementType.Transaction}
                             AND e.""{nameof(StatementEntryEntity.StatementDirectionId)}"" = {(int)StatementDirection.Credit}
                        THEN e.""{nameof(StatementEntryEntity.Amount)}""

                        WHEN e.""{nameof(StatementEntryEntity.StatementTypeId)}"" = {(int)StatementType.Transaction}
                             AND e.""{nameof(StatementEntryEntity.StatementDirectionId)}"" = {(int)StatementDirection.Debit}
                        THEN -e.""{nameof(StatementEntryEntity.Amount)}""

                        WHEN e.""{nameof(StatementEntryEntity.StatementTypeId)}"" = {(int)StatementType.Hold}
                        THEN -e.""{nameof(StatementEntryEntity.Amount)}""
                    END
                ) OVER (
                    PARTITION BY e.""{nameof(StatementEntryEntity.AccountId)}""
                    ORDER BY e.""{nameof(StatementEntryEntity.ActionedAt)}"", e.""{nameof(StatementEntryEntity.StatementEntryId)}""
                ) AS ""{nameof(StatementEntryEntity.AvailableBalance)}""
            FROM
            (
                SELECT
                    t.""{nameof(TransactionEntity.TransactionId)}"" AS ""{nameof(StatementEntryEntity.StatementEntryId)}"",
                    t.""{nameof(TransactionEntity.AccountId)}"" AS ""{nameof(StatementEntryEntity.AccountId)}"",
                    t.""{nameof(TransactionEntity.CurrencyCode)}"" AS ""{nameof(StatementEntryEntity.CurrencyCode)}"",
                    t.""{nameof(TransactionEntity.PostedAt)}"" AS ""{nameof(StatementEntryEntity.ActionedAt)}"",
                    t.""{nameof(TransactionEntity.Amount)}"" AS ""{nameof(StatementEntryEntity.Amount)}"",
                    {(int)StatementType.Transaction} AS ""{nameof(StatementEntryEntity.StatementTypeId)}"",
                    t.""{nameof(TransactionEntity.TransactionDirectionId)}"" AS ""{nameof(StatementEntryEntity.StatementDirectionId)}"",
                    t.""{nameof(TransactionEntity.Description)}"" AS ""{nameof(StatementEntryEntity.Description)}"",
                    t.""{nameof(TransactionEntity.Reference)}"" AS ""{nameof(StatementEntryEntity.Reference)}"",
                    CASE t.""{nameof(TransactionEntity.TransactionStatusId)}""
                        WHEN {(int)TransactionStatus.Draft} THEN {(int)StatementStatus.TransactiomDraft}
                        WHEN {(int)TransactionStatus.Posted} THEN {(int)StatementStatus.TramsactiomPosted}
                        WHEN {(int)TransactionStatus.Reversed} THEN {(int)StatementStatus.TransactionReversed}
                        ELSE {(int)StatementStatus.Unknown}
                    END AS ""{nameof(StatementEntryEntity.StatementStatusId)}""
                FROM ""Transactions"" t
                WHERE t.""{nameof(TransactionEntity.AccountId)}"" = @AccountId
                  AND t.""{nameof(TransactionEntity.PostedAt)}"" BETWEEN @FromDate AND @ToDate
                  AND t.""{nameof(TransactionEntity.IsDeleted)}"" = false
                  AND t.""{nameof(TransactionEntity.TransactionDirectionId)}"" = {(int)StatementDirection.Credit}
                  AND t.""{nameof(TransactionEntity.TransactionStatusId)}"" IN ({(int)TransactionStatus.Posted}, {(int)TransactionStatus.Reversed})

                UNION ALL

                SELECT
                    t.""{nameof(TransactionEntity.TransactionId)}"" AS ""{nameof(StatementEntryEntity.StatementEntryId)}"",
                    t.""{nameof(TransactionEntity.AccountId)}"" AS ""{nameof(StatementEntryEntity.AccountId)}"",
                    t.""{nameof(TransactionEntity.CurrencyCode)}"" AS ""{nameof(StatementEntryEntity.CurrencyCode)}"",
                    t.""{nameof(TransactionEntity.CreatedAt)}"" AS ""{nameof(StatementEntryEntity.ActionedAt)}"",
                    t.""{nameof(TransactionEntity.Amount)}"" AS ""{nameof(StatementEntryEntity.Amount)}"",
                    {(int)StatementType.Transaction} AS ""{nameof(StatementEntryEntity.StatementTypeId)}"",
                    t.""{nameof(TransactionEntity.TransactionDirectionId)}"" AS ""{nameof(StatementEntryEntity.StatementDirectionId)}"",
                    t.""{nameof(TransactionEntity.Description)}"" AS ""{nameof(StatementEntryEntity.Description)}"",
                    t.""{nameof(TransactionEntity.Reference)}"" AS ""{nameof(StatementEntryEntity.Reference)}"",
                    CASE t.""{nameof(TransactionEntity.TransactionStatusId)}""
                        WHEN {(int)TransactionStatus.Draft} THEN {(int)StatementStatus.TransactiomDraft}
                        WHEN {(int)TransactionStatus.Posted} THEN {(int)StatementStatus.TramsactiomPosted}
                        WHEN {(int)TransactionStatus.Reversed} THEN {(int)StatementStatus.TransactionReversed}
                        ELSE {(int)StatementStatus.Unknown}
                    END AS ""{nameof(StatementEntryEntity.StatementStatusId)}""
                FROM ""Transactions"" t
                WHERE t.""{nameof(TransactionEntity.AccountId)}"" = @AccountId
                  AND t.""{nameof(TransactionEntity.CreatedAt)}"" BETWEEN @FromDate AND @ToDate
                  AND t.""{nameof(TransactionEntity.IsDeleted)}"" = false
                  AND t.""{nameof(TransactionEntity.TransactionDirectionId)}"" = {(int)StatementDirection.Debit}

                UNION ALL

                SELECT
                    h.""{nameof(HoldEntity.HoldId)}"" AS ""{nameof(StatementEntryEntity.StatementEntryId)}"",
                    h.""{nameof(HoldEntity.AccountId)}"" AS ""{nameof(StatementEntryEntity.AccountId)}"",
                    h.""{nameof(HoldEntity.CurrencyCode)}"" AS ""{nameof(StatementEntryEntity.CurrencyCode)}"",
                    h.""{nameof(HoldEntity.CreatedAt)}"" AS ""{nameof(StatementEntryEntity.ActionedAt)}"",
                    h.""{nameof(HoldEntity.Amount)}"" AS ""{nameof(StatementEntryEntity.Amount)}"",
                    {(int)StatementType.Hold} AS ""{nameof(StatementEntryEntity.StatementTypeId)}"",
                    {(int)StatementDirection.Debit} AS ""{nameof(StatementEntryEntity.StatementDirectionId)}"",
                    h.""{nameof(HoldEntity.Description)}"" AS ""{nameof(StatementEntryEntity.Description)}"",
                    h.""{nameof(HoldEntity.Reference)}"" AS ""{nameof(StatementEntryEntity.Reference)}"",
                    CASE h.""{nameof(HoldEntity.HoldStatusId)}""
                        WHEN {(int)HoldStatus.Active}   THEN {(int)StatementStatus.HoldActive}
                        WHEN {(int)HoldStatus.Released} THEN {(int)StatementStatus.HoldReleased}
                        WHEN {(int)HoldStatus.Settled}  THEN {(int)StatementStatus.HoldSettled}
                        WHEN {(int)HoldStatus.Expired}  THEN {(int)StatementStatus.HoldExpired}
                        ELSE {(int)StatementStatus.Unknown}
                    END AS ""{nameof(StatementEntryEntity.StatementStatusId)}""
                FROM ""Holds"" h
                WHERE h.""{nameof(HoldEntity.AccountId)}"" = @AccountId
                  AND h.""{nameof(HoldEntity.CreatedAt)}"" BETWEEN @FromDate AND @ToDate
                  AND h.""{nameof(HoldEntity.IsDeleted)}"" = false
                  AND h.""{nameof(HoldEntity.HoldStatusId)}"" = {(int)HoldStatus.Active}
            ) e";

        var query = dbContext.Database.SqlQueryRaw<StatementEntryEntity>(sql, [
            new NpgsqlParameter("@AccountId", NpgsqlDbType.Uuid)
            {
                Value = (Guid)getStatementRequest.AccountId
            },
            new NpgsqlParameter("@FromDate", NpgsqlDbType.TimestampTz)
            {
                Value = getStatementRequest.DateRange.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
            },
            new NpgsqlParameter("@ToDate", NpgsqlDbType.TimestampTz)
            {
                Value = getStatementRequest.DateRange.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc)
            }
        ]);

        if (getStatementRequest.Direction.HasValue)
        {
            query = query.Where(x => x.StatementDirectionId == (int)getStatementRequest.Direction);
        }

        return query;
    }
}