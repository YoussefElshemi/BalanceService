using Core.Constants;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class StatementRepository(ApplicationDbContext dbContext) : IStatementRepository
{
    public async Task<AvailableBalance> GetAvailableBalanceAtAsync(BalanceRequest balanceRequest, CancellationToken cancellationToken)
    {
        var result = await dbContext.Transactions
            .Where(x => x.IsDeleted == false)
            .Where(x => x.AccountId == balanceRequest.AccountId)
            .Where(x => DateOnly.FromDateTime(x.PostedAt!.Value.UtcDateTime) <= balanceRequest.Date)
            .Where(x => x.TransactionStatusId == (int)TransactionStatus.Posted ||
                        x.TransactionStatusId == (int)TransactionStatus.Reversed)
            .GroupBy(x => 1)
            .Select(x => new
            {
                LedgerBalance = x.Sum(y =>
                    y.TransactionDirectionId == (int)TransactionDirection.Credit ? y.Amount : -y.Amount),
                HoldBalance = dbContext.Holds
                    .Where(z => z.IsDeleted == false)
                    .Where(z => z.AccountId == balanceRequest.AccountId)
                    .Where(z => DateOnly.FromDateTime(z.CreatedAt.UtcDateTime) <= balanceRequest.Date)
                    .Where(z => z.HoldStatusId == (int)HoldStatus.Active)
                    .Sum(z => z.Amount)
            })
            .SingleOrDefaultAsync(cancellationToken);

        return new AvailableBalance(result?.LedgerBalance - result?.HoldBalance ?? 0);
    }

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
        var fromDate = getStatementRequest.DateRange.From.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).ToString(DateTimeConstants.DateTimeFormat);
        var toDate = getStatementRequest.DateRange.To.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc).ToString(DateTimeConstants.DateTimeFormat);

        var sql = $@"
            SELECT
                e.*,
                SUM(
                    CASE
                        WHEN e.""{nameof(StatementEntryEntity.StatementTypeId)}"" = {(int)StatementType.Transaction} AND e.""{nameof(StatementEntryEntity.StatementDirectionId)}"" = {(int)StatementDirection.Credit} THEN e.""{nameof(StatementEntryEntity.Amount)}""
                        WHEN e.""{nameof(StatementEntryEntity.StatementTypeId)}"" = {(int)StatementType.Transaction} AND e.""{nameof(StatementEntryEntity.StatementDirectionId)}"" <> {(int)StatementDirection.Credit} THEN -e.""{nameof(StatementEntryEntity.Amount)}""
                        WHEN e.""{nameof(StatementEntryEntity.StatementTypeId)}"" = {(int)StatementType.Hold} THEN -e.""{nameof(StatementEntryEntity.Amount)}""
                    END
                ) OVER (PARTITION BY e.""{nameof(StatementEntryEntity.AccountId)}"" ORDER BY e.""{nameof(StatementEntryEntity.ActionedAt)}"", e.""{nameof(StatementEntryEntity.StatementEntryId)}"") AS ""{nameof(StatementEntryEntity.AvailableBalance)}""
            FROM
            (
                SELECT
                    t.""{nameof(TransactionEntity.TransactionId)}"" AS ""{nameof(StatementEntryEntity.StatementEntryId)}"",
                    t.""{nameof(TransactionEntity.AccountId)}"" AS ""{nameof(StatementEntryEntity.AccountId)}"",
                    t.""{nameof(TransactionEntity.CurrencyCode)}"" AS ""{nameof(StatementEntryEntity.CurrencyCode)}"",
                    t.""{nameof(TransactionEntity.PostedAt)}"" as ""{nameof(StatementEntryEntity.ActionedAt)}"",
                    t.""{nameof(TransactionEntity.Amount)}"" AS ""{nameof(StatementEntryEntity.Amount)}"",
                    {(int)StatementType.Transaction} AS ""{nameof(StatementEntryEntity.StatementTypeId)}"",
                    t.""{nameof(TransactionEntity.TransactionDirectionId)}"" AS ""{nameof(StatementEntryEntity.StatementDirectionId)}"",
                    t.""{nameof(TransactionEntity.Description)}"" AS ""{nameof(StatementEntryEntity.Description)}"",
                    t.""{nameof(TransactionEntity.Reference)}"" AS ""{nameof(StatementEntryEntity.Reference)}""
                FROM ""Transactions"" t
                WHERE t.""{nameof(TransactionEntity.AccountId)}"" = '{getStatementRequest.AccountId}'
                  AND t.""{nameof(TransactionEntity.PostedAt)}"" BETWEEN '{fromDate}' AND '{toDate}'
                  AND t.""{nameof(TransactionEntity.IsDeleted)}"" = false
                  AND t.""{nameof(TransactionEntity.TransactionStatusId)}"" in ({(int)TransactionStatus.Posted}, {(int)TransactionStatus.Reversed})

                UNION ALL

                SELECT
                    h.""{nameof(HoldEntity.HoldId)}"" AS ""{nameof(StatementEntryEntity.StatementEntryId)}"",
                    h.""{nameof(HoldEntity.AccountId)}"" AS ""{nameof(StatementEntryEntity.AccountId)}"",
                    h.""{nameof(HoldEntity.CurrencyCode)}"" AS ""{nameof(StatementEntryEntity.CurrencyCode)}"",
                    h.""{nameof(HoldEntity.CreatedAt)}"" as ""{nameof(StatementEntryEntity.ActionedAt)}"",
                    h.""{nameof(HoldEntity.Amount)}"" AS ""{nameof(StatementEntryEntity.Amount)}"",
                    {(int)StatementType.Hold} AS ""{nameof(StatementEntryEntity.StatementTypeId)}"",
                    {(int)StatementDirection.Debit} AS ""{nameof(StatementEntryEntity.StatementDirectionId)}"",
                    h.""{nameof(HoldEntity.Description)}"" AS ""{nameof(StatementEntryEntity.Description)}"",
                    h.""{nameof(HoldEntity.Reference)}"" AS ""{nameof(StatementEntryEntity.Reference)}""
                FROM ""Holds"" h
                WHERE h.""{nameof(HoldEntity.AccountId)}"" = '{getStatementRequest.AccountId}'
                  AND h.""{nameof(HoldEntity.CreatedAt)}"" BETWEEN '{fromDate}' AND '{toDate}'
                  AND h.""{nameof(HoldEntity.IsDeleted)}"" = false
                  AND h.""{nameof(HoldEntity.HoldStatusId)}"" = {(int)HoldStatus.Active}
            ) e";

        var query = dbContext.Database.SqlQueryRaw<StatementEntryEntity>(sql);

        if (getStatementRequest.Direction.HasValue)
        {
            query = query.Where(x => x.StatementDirectionId == (int)getStatementRequest.Direction);
        }

        return query;
    }
}