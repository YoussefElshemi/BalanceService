using Core.Models;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions;

public static class InterestProductAccountLinkEntityQueryableExtensions
{
    public static IQueryable<InterestProductAccountLinkEntity> BuildSearchQuery(
        this IQueryable<InterestProductAccountLinkEntity> query,
        QueryInterestProductAccountLinksRequest queryInterestProductAccountLinksRequest)
    {
        query = query
            .AsNoTracking()
            .Where(x => x.IsDeleted == false);

        if (queryInterestProductAccountLinksRequest.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive ==  queryInterestProductAccountLinksRequest.IsActive.Value);
        }

        return query;
    }
}