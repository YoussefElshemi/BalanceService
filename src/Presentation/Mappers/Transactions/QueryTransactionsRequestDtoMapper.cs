using Core.Models;
using Core.ValueObjects;
using Presentation.Models.Transactions;

namespace Presentation.Mappers.Transactions;

public static class QueryTransactionsRequestDtoMapper
{
    public static QueryTransactionsRequest ToModel(this QueryTransactionsRequestDto queryTransactionsRequestDto)
    {
        return new QueryTransactionsRequest
        {
            PageSize = new PageSize(queryTransactionsRequestDto.PageSize),
            PageNumber = new PageNumber(queryTransactionsRequestDto.PageNumber),
            AccountId = queryTransactionsRequestDto.AccountId.HasValue
                ? new AccountId(queryTransactionsRequestDto.AccountId.Value)
                : null,
            CurrencyCode = queryTransactionsRequestDto.CurrencyCode,
            Amount = queryTransactionsRequestDto.Amount.HasValue ?
                new TransactionAmount(queryTransactionsRequestDto.Amount.Value)
                : null,
            Direction = queryTransactionsRequestDto.Direction,
            PostedDateRange = queryTransactionsRequestDto.FromPostedDate.HasValue ||
                              queryTransactionsRequestDto.ToPostedDate.HasValue
                ? new Range<DateOnly?>
                {
                    From = queryTransactionsRequestDto.FromPostedDate,
                    To = queryTransactionsRequestDto.ToPostedDate
                }
                : null,
            Type = queryTransactionsRequestDto.Type,
            Statuses = queryTransactionsRequestDto.Status.HasValue
                ? [queryTransactionsRequestDto.Status.Value]
                : null,
            Source = queryTransactionsRequestDto.Source,
            Description =  !string.IsNullOrWhiteSpace(queryTransactionsRequestDto.Description)
                ? new TransactionDescription(queryTransactionsRequestDto.Description)
                : null,
            Reference =  !string.IsNullOrWhiteSpace(queryTransactionsRequestDto.Reference)
                ? new TransactionReference(queryTransactionsRequestDto.Reference)
                : null
        };
    }
}