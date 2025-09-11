using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

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
            Type = queryHoldsRequestDto.Type,
            Status = queryHoldsRequestDto.Status,
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