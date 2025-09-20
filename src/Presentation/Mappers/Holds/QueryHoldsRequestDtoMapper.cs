using Core.Models;
using Core.ValueObjects;
using Presentation.Models.Holds;

namespace Presentation.Mappers.Holds;

public static class QueryHoldsRequestDtoMapper
{
    public static QueryHoldsRequest ToModel(this QueryHoldsRequestDto queryHoldsRequestDto)
    {
        return new QueryHoldsRequest
        {
            PageSize = new PageSize(queryHoldsRequestDto.PageSize),
            PageNumber = new PageNumber(queryHoldsRequestDto.PageNumber),
            AccountId = queryHoldsRequestDto.AccountId.HasValue
                ? new AccountId(queryHoldsRequestDto.AccountId.Value)
                : null,
            CurrencyCode = queryHoldsRequestDto.CurrencyCode,
            Amount = queryHoldsRequestDto.Amount.HasValue ?
                new HoldAmount(queryHoldsRequestDto.Amount.Value)
                : null,
            SettledTransactionId = queryHoldsRequestDto.SettledTransactionId.HasValue
                ? new TransactionId(queryHoldsRequestDto.SettledTransactionId.Value)
                : null,
            ExpiresAt = queryHoldsRequestDto.ExpiresAt.HasValue
                ? new ExpiresAt(queryHoldsRequestDto.ExpiresAt.Value)
                : null,
            CreatedAtRange = queryHoldsRequestDto.FromCreatedAt.HasValue ||
                             queryHoldsRequestDto.ToCreatedAt.HasValue
                ? new Range<CreatedAt?>
                {
                    From = queryHoldsRequestDto.FromCreatedAt.HasValue ?
                        new CreatedAt(queryHoldsRequestDto.FromCreatedAt.Value)
                        : null,
                    To = queryHoldsRequestDto.ToCreatedAt.HasValue ?
                        new CreatedAt(queryHoldsRequestDto.ToCreatedAt.Value)
                        : null,
                }
                : null,
            Type = queryHoldsRequestDto.Type,
            Statuses = queryHoldsRequestDto.Status.HasValue ?
                [queryHoldsRequestDto.Status.Value]
                : null,
            Source = queryHoldsRequestDto.Source,
            Description =  !string.IsNullOrWhiteSpace(queryHoldsRequestDto.Description)
                ? new HoldDescription(queryHoldsRequestDto.Description)
                : null,
            Reference =  !string.IsNullOrWhiteSpace(queryHoldsRequestDto.Reference)
                ? new HoldReference(queryHoldsRequestDto.Reference)
                : null
        };
    }
}