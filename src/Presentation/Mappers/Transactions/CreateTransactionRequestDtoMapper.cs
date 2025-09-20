using Core.Enums;
using Core.Models;
using Core.ValueObjects;
using Presentation.Models.Transactions;

namespace Presentation.Mappers.Transactions;

public static class CreateTransactionRequestDtoMapper
{
    public static CreateTransactionRequest ToModel(this CreateTransactionRequestDto createTransactionRequestDto)
    {
        return new CreateTransactionRequest
        {
            AccountId = new AccountId(createTransactionRequestDto.AccountId),
            Amount = new TransactionAmount(createTransactionRequestDto.Amount),
            CurrencyCode = createTransactionRequestDto.CurrencyCode,
            Direction = createTransactionRequestDto.Direction,
            IdempotencyKey = new IdempotencyKey(createTransactionRequestDto.IdempotencyKey),
            Status = TransactionStatus.Draft,
            PostedAt = null,
            Type = createTransactionRequestDto.Type,
            Source = TransactionSource.Api,
            Description = !string.IsNullOrWhiteSpace(createTransactionRequestDto.Description)
                ? new TransactionDescription(createTransactionRequestDto.Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(createTransactionRequestDto.Reference)
                ? new TransactionReference(createTransactionRequestDto.Reference)
                : null,
            CreatedBy = new Username(createTransactionRequestDto.Username)
        };
    }
}