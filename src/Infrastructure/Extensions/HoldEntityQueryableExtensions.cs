using Core.Models;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions;

public static class HoldEntityQueryableExtensions
{
    public static IQueryable<HoldEntity> BuildSearchQuery(
        this IQueryable<HoldEntity> query,
        QueryHoldsRequest queryHoldsRequest)
    {
        query = query
            .AsNoTracking()
            .Where(x => x.IsDeleted == false);

        if (queryHoldsRequest.AccountId.HasValue)
        {
            query = query.Where(x => x.AccountId == queryHoldsRequest.AccountId);
        }

        if (queryHoldsRequest.CurrencyCode.HasValue)
        {
            query = query.Where(x => x.CurrencyCode == queryHoldsRequest.CurrencyCode.ToString());
        }

        if (queryHoldsRequest.Amount.HasValue)
        {
            query = query.Where(x => x.Amount == queryHoldsRequest.Amount);
        }

        if (queryHoldsRequest.SettledTransactionId.HasValue)
        {
            query = query.Where(x => x.SettledTransactionId == queryHoldsRequest.SettledTransactionId);
        }

        if (queryHoldsRequest.ExpiresAt.HasValue)
        {
            query = query.Where(x => x.ExpiresAt == queryHoldsRequest.ExpiresAt);
        }

        if (queryHoldsRequest.CreatedAtRange.HasValue)
        {
            if (queryHoldsRequest.CreatedAtRange.Value.From.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= queryHoldsRequest.CreatedAtRange.Value.From.Value);    
            }

            if (queryHoldsRequest.CreatedAtRange.Value.To.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= queryHoldsRequest.CreatedAtRange.Value.To.Value);    
            }
        }

        if (queryHoldsRequest.Type.HasValue)
        {
            query = query.Where(x => x.HoldTypeId == (int)queryHoldsRequest.Type);
        }

        if (queryHoldsRequest.Statuses != null && queryHoldsRequest.Statuses.Length != 0)
        {
            query = query.Where(x => queryHoldsRequest.Statuses.Select(y => (int)y).Contains(x.HoldStatusId));
        }

        if (queryHoldsRequest.Source.HasValue)
        {
            query = query.Where(x => x.HoldSourceId == (int)queryHoldsRequest.Source);
        }

        if (queryHoldsRequest.Description.HasValue)
        {
            query = query.Where(x => x.Description != null && EF.Functions.ILike(x.Description, $"%{queryHoldsRequest.Description}%"));
        }

        if (queryHoldsRequest.Reference.HasValue)
        {
            query = query.Where(x => x.Reference != null && EF.Functions.ILike(x.Reference, $"%{queryHoldsRequest.Reference}%"));
        }

        return query;
    }
}