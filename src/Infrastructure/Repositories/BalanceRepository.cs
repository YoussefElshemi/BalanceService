using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Constants;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;

namespace Infrastructure.Repositories;

public class BalanceRepository(ApplicationDbContext dbContext) : IBalanceRepository
{
    public async Task<AvailableBalance> GetAvailableBalanceAsync(BalanceRequest balanceRequest, CancellationToken cancellationToken)
    {
        var sql = $@"
        WITH latest_transactions AS (
           SELECT *
           FROM (
               SELECT ""{nameof(TransactionHistoryEntity.TransactionDirectionId)}"",
                      ""{nameof(TransactionHistoryEntity.TransactionStatusId)}"",
                      ""{nameof(TransactionHistoryEntity.Amount)}"",
                      ""{nameof(TransactionHistoryEntity.IsDeleted)}"",
                      ROW_NUMBER() OVER(PARTITION BY ""{nameof(TransactionHistoryEntity.TransactionId)}"" ORDER BY ""{nameof(TransactionHistoryEntity.Timestamp)}"" DESC) AS rn
               FROM ""{TableNames.TransactionHistory}""
               WHERE ""{nameof(TransactionHistoryEntity.AccountId)}"" = @AccountId AND
                     ""{nameof(TransactionHistoryEntity.Timestamp)}"" <= @AsOf
           ) t
           WHERE t.rn = 1
        ),
        latest_holds AS (
           SELECT *
           FROM (
               SELECT ""{nameof(HoldHistoryEntity.HoldStatusId)}"",
                      ""{nameof(HoldHistoryEntity.Amount)}"",
                      ""{nameof(HoldHistoryEntity.IsDeleted)}"",
                      ROW_NUMBER() OVER(PARTITION BY ""{nameof(HoldHistoryEntity.HoldId)}"" ORDER BY ""{nameof(HoldHistoryEntity.Timestamp)}"" DESC) AS rn
               FROM ""{TableNames.HoldHistory}""
               WHERE ""{nameof(HoldHistoryEntity.AccountId)}"" = @AccountId AND
                     ""{nameof(HoldHistoryEntity.Timestamp)}"" <= @AsOf
           ) h
           WHERE h.rn = 1
        )
        SELECT
           COALESCE(
               (SELECT SUM(
                   CASE
                       WHEN ""{nameof(TransactionHistoryEntity.TransactionDirectionId)}"" = {(int)TransactionDirection.Credit} AND ""{nameof(TransactionHistoryEntity.TransactionStatusId)}"" IN ({(int)TransactionStatus.Posted}, {(int)TransactionStatus.Reversed}) THEN ""{nameof(TransactionHistoryEntity.Amount)}""
                       WHEN ""{nameof(TransactionHistoryEntity.TransactionDirectionId)}"" = {(int)TransactionDirection.Debit} THEN -""{nameof(TransactionHistoryEntity.Amount)}""
                       ELSE 0
                   END
               )
               FROM latest_transactions
               WHERE ""{nameof(TransactionHistoryEntity.IsDeleted)}"" = false), 0)
           -
           COALESCE(
               (SELECT SUM(""{nameof(HoldHistoryEntity.Amount)}"") 
                FROM latest_holds 
                WHERE ""{nameof(HoldHistoryEntity.HoldStatusId)}"" = {(int)HoldStatus.Active} AND
                      ""{nameof(HoldHistoryEntity.IsDeleted)}"" = false), 0)
        AS ""Value""
        ";

        var availableBalance = await dbContext.Database.SqlQueryRaw<decimal>(sql, [
            new NpgsqlParameter("@AccountId", NpgsqlDbType.Uuid)
            {
                Value = (Guid)balanceRequest.AccountId
            },
            new NpgsqlParameter("@AsOf", NpgsqlDbType.TimestampTz)
            {
                Value = ((DateTimeOffset)balanceRequest.Timestamp).ToUniversalTime()
            }
        ]).SingleAsync(cancellationToken);

        return new AvailableBalance(availableBalance);
    }

    public async Task<AvailableBalance> GetEligibleBalanceAsync(
        BalanceRequest balanceRequest,
        TimeSpan lag,
        CancellationToken cancellationToken)
    {
        var sql = $@"
        WITH latest_transactions AS (
            SELECT *
            FROM (
                SELECT ""{nameof(TransactionHistoryEntity.TransactionDirectionId)}"",
                       ""{nameof(TransactionHistoryEntity.TransactionStatusId)}"",
                       ""{nameof(TransactionHistoryEntity.Amount)}"",
                       ""{nameof(TransactionHistoryEntity.IsDeleted)}"",
                       ""{nameof(TransactionHistoryEntity.Timestamp)}"",
                       ROW_NUMBER() OVER(PARTITION BY ""{nameof(TransactionHistoryEntity.TransactionId)}"" ORDER BY ""{nameof(TransactionHistoryEntity.Timestamp)}"" DESC) AS rn
                FROM ""{TableNames.TransactionHistory}""
                WHERE ""{nameof(TransactionHistoryEntity.AccountId)}"" = @AccountId
                  AND ""{nameof(TransactionHistoryEntity.Timestamp)}"" <= @AsOf
            ) t
            WHERE t.rn = 1
        ),
        latest_holds AS (
            SELECT *
            FROM (
                SELECT ""{nameof(HoldHistoryEntity.HoldStatusId)}"",
                       ""{nameof(HoldHistoryEntity.Amount)}"",
                       ""{nameof(HoldHistoryEntity.IsDeleted)}"",
                       ROW_NUMBER() OVER(PARTITION BY ""{nameof(HoldHistoryEntity.HoldId)}"" ORDER BY ""{nameof(HoldHistoryEntity.Timestamp)}"" DESC) AS rn
                FROM ""{TableNames.HoldHistory}""
                WHERE ""{nameof(HoldHistoryEntity.AccountId)}"" = @AccountId
                  AND ""{nameof(HoldHistoryEntity.Timestamp)}"" <= @AsOf
            ) h
            WHERE h.rn = 1
        )
        SELECT
            -- Mature credits
            COALESCE((
                SELECT SUM(""{nameof(TransactionHistoryEntity.Amount)}"")
                FROM latest_transactions
                WHERE ""{nameof(TransactionHistoryEntity.IsDeleted)}"" = false
                  AND ""{nameof(TransactionHistoryEntity.TransactionDirectionId)}"" = {(int)TransactionDirection.Credit}
                  AND ""{nameof(TransactionHistoryEntity.TransactionStatusId)}"" IN ({(int)TransactionStatus.Posted}, {(int)TransactionStatus.Reversed})
                  AND ""{nameof(TransactionHistoryEntity.Timestamp)}"" <= @MatureCutoff
            ), 0)
            -- All debits
            - COALESCE((
                SELECT SUM(""{nameof(TransactionHistoryEntity.Amount)}"")
                FROM latest_transactions
                WHERE ""{nameof(TransactionHistoryEntity.IsDeleted)}"" = false
                  AND ""{nameof(TransactionHistoryEntity.TransactionDirectionId)}"" = {(int)TransactionDirection.Debit}
            ), 0)
            -- Active holds
            - COALESCE((
                SELECT SUM(""{nameof(HoldHistoryEntity.Amount)}"")
                FROM latest_holds
                WHERE ""{nameof(HoldHistoryEntity.IsDeleted)}"" = false
                  AND ""{nameof(HoldHistoryEntity.HoldStatusId)}"" = {(int)HoldStatus.Active}
            ), 0)
        AS ""Value""
        ";

        var availableBalance = await dbContext.Database.SqlQueryRaw<decimal>(sql, [
            new NpgsqlParameter("@AccountId", NpgsqlDbType.Uuid)
            {
                Value = (Guid)balanceRequest.AccountId
            },
            new NpgsqlParameter("@AsOf", NpgsqlDbType.TimestampTz)
            {
                Value = ((DateTimeOffset)balanceRequest.Timestamp).ToUniversalTime()
            },
            new NpgsqlParameter("@MatureCutoff", NpgsqlDbType.TimestampTz)
            {
                Value = ((DateTimeOffset)balanceRequest.Timestamp - lag).ToUniversalTime()
            }
        ]).SingleAsync(cancellationToken);

        return new AvailableBalance(availableBalance);
    }
}