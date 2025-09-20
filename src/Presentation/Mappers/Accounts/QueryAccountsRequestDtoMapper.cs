using System.Text.Json;
using Core.Models;
using Core.ValueObjects;
using Presentation.Models.Accounts;

namespace Presentation.Mappers.Accounts;

public static class QueryAccountsRequestDtoMapper
{
    public static QueryAccountsRequest ToModel(this QueryAccountsRequestDto queryAccountsRequestDto)
    {
        return new QueryAccountsRequest
        {
            PageSize = new PageSize(queryAccountsRequestDto.PageSize),
            PageNumber = new PageNumber(queryAccountsRequestDto.PageNumber),
            AccountName = !string.IsNullOrWhiteSpace(queryAccountsRequestDto.AccountName)
                ? new AccountName(queryAccountsRequestDto.AccountName)
                : null,
            CurrencyCode = queryAccountsRequestDto.CurrencyCode,
            AccountType = queryAccountsRequestDto.AccountType,
            ParentAccountId = queryAccountsRequestDto.ParentAccountId.HasValue
                ? new AccountId(queryAccountsRequestDto.ParentAccountId.Value)
                : null,
            ParentAccountName = !string.IsNullOrWhiteSpace(queryAccountsRequestDto.ParentAccountName)
                ? new AccountName(queryAccountsRequestDto.ParentAccountName)
                : null,
            Metadata =  !string.IsNullOrWhiteSpace(queryAccountsRequestDto.Metadata)
                ? JsonDocument.Parse(queryAccountsRequestDto.Metadata)
                : null
        };
    }
}