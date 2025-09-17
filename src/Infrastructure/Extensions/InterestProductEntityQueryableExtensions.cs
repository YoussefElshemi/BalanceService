using Core.Models;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions;

public static class InterestProductEntityQueryableExtensions
{
    public static IQueryable<InterestProductEntity> BuildSearchQuery(
        this IQueryable<InterestProductEntity> query,
        QueryInterestProductsRequest queryInterestProductsRequest)
    {
        query = query
            .AsNoTracking()
            .Where(x => x.IsDeleted == false);
      
        if (queryInterestProductsRequest.Name.HasValue)
        {
            query = query.Where(x => EF.Functions.ILike(x.Name, $"%{queryInterestProductsRequest.Name}%"));
        }

        if (queryInterestProductsRequest.InterestPayoutFrequency.HasValue)
        {
            query = query.Where(x => x.InterestPayoutFrequencyId == (int)queryInterestProductsRequest.InterestPayoutFrequency);
        }

        return query;
    }
}