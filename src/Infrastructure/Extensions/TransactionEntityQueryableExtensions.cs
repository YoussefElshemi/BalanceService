using Core.Models;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions;

public static class TransactionEntityQueryableExtensions
{
    public static IQueryable<TransactionEntity> BuildSearchQuery(
        this IQueryable<TransactionEntity> query,
        QueryTransactionsRequest queryTransactionsRequest)
    {
        query = query
            .AsNoTracking()
            .Where(x => x.IsDeleted == false);

        if (queryTransactionsRequest.AccountId.HasValue)
        {
            query = query.Where(x => x.AccountId == queryTransactionsRequest.AccountId);
        }

        if (queryTransactionsRequest.CurrencyCode.HasValue)
        {
            query = query.Where(x => x.CurrencyCode == queryTransactionsRequest.CurrencyCode.ToString());
        }

        if (queryTransactionsRequest.Amount.HasValue)
        {
            query = query.Where(x => x.Amount == queryTransactionsRequest.Amount);
        }

        if (queryTransactionsRequest.Direction.HasValue)
        {
            query = query.Where(x => x.TransactionDirectionId == (int)queryTransactionsRequest.Direction);
        }

        if (queryTransactionsRequest.PostedDateRange.HasValue)
        {
            if (queryTransactionsRequest.PostedDateRange.Value.From.HasValue)
            {
                query = query.Where(x => DateOnly.FromDateTime(x.PostedAt!.Value.UtcDateTime) >= queryTransactionsRequest.PostedDateRange.Value.From.Value);    
            }

            if (queryTransactionsRequest.PostedDateRange.Value.To.HasValue)
            {
                query = query.Where(x => DateOnly.FromDateTime(x.PostedAt!.Value.UtcDateTime) <= queryTransactionsRequest.PostedDateRange.Value.To.Value);    
            }
        }

        if (queryTransactionsRequest.Type.HasValue)
        {
            query = query.Where(x => x.TransactionTypeId == (int)queryTransactionsRequest.Type);
        }

        if (queryTransactionsRequest.Statuses != null && queryTransactionsRequest.Statuses.Length != 0)
        {
            query = query.Where(x => queryTransactionsRequest.Statuses.Select(y => (int)y).Contains(x.TransactionStatusId));
        }

        if (queryTransactionsRequest.Source.HasValue)
        {
            query = query.Where(x => x.TransactionSourceId == (int)queryTransactionsRequest.Source);
        }

        if (queryTransactionsRequest.Description.HasValue)
        {
            query = query.Where(x => x.Description != null && EF.Functions.ILike(x.Description, $"%{queryTransactionsRequest.Description}%"));
        }

        if (queryTransactionsRequest.Reference.HasValue)
        {
            query = query.Where(x => x.Reference != null && EF.Functions.ILike(x.Reference, $"%{queryTransactionsRequest.Reference}%"));
        }

        return query;
    }
}