using Core.Models;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Extensions;

public static class AccountEntityQueryableExtensions
{
    public static IQueryable<AccountEntity> BuildSearchQuery(
        this IQueryable<AccountEntity> query,
        QueryAccountsRequest queryAccountsRequest)
    {
        query = query
            .AsNoTracking()
            .Where(x => x.IsDeleted == false);

        if (queryAccountsRequest.AccountName.HasValue)
        {
            query = query.Where(x => EF.Functions.ILike(x.AccountName, $"%{queryAccountsRequest.AccountName}%"));
        }

        if (queryAccountsRequest.CurrencyCode.HasValue)
        {
            query = query.Where(x => x.CurrencyCode == queryAccountsRequest.CurrencyCode.ToString());
        }

        if (queryAccountsRequest.AccountType.HasValue)
        {
            query = query.Where(x => x.AccountTypeId == (int)queryAccountsRequest.AccountType);
        }

        if (queryAccountsRequest.ParentAccountId.HasValue)
        {
            query = query.Where(x => x.ParentAccountId == queryAccountsRequest.ParentAccountId);
        }

        if (queryAccountsRequest.ParentAccountName.HasValue)
        {
            query = query.Where(x =>
                x.ParentAccountEntity != null && EF.Functions.ILike(x.ParentAccountEntity.AccountName,
                    $"%{queryAccountsRequest.ParentAccountName}%"));
        }

        if (queryAccountsRequest.Metadata != null)
        {
            query = query.Where(x =>
                x.Metadata != null && EF.Functions.JsonContains(x.Metadata, queryAccountsRequest.Metadata));
        }

        return query;
    }
}