using Core.Enums;
using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class CreateTransferRequestDtoMapper
{
    public static CreateTransferRequest ToModel(this CreateTransferRequestDto createTransferRequestDto, string username)
    {
        return new CreateTransferRequest
        {
            DebitAccountId = new AccountId(createTransferRequestDto.DebitAccountId),
            CreditAccountId = new AccountId(createTransferRequestDto.CreditAccountId),
            Amount = new TransferAmount(createTransferRequestDto.Amount),
            CurrencyCode = createTransferRequestDto.CurrencyCode,
            DebitIdempotencyKey = new IdempotencyKey(createTransferRequestDto.DebitIdempotencyKey),
            CreditIdempotencyKey = new IdempotencyKey(createTransferRequestDto.CreditIdempotencyKey),
            Source = TransferSource.Api,
            Description = !string.IsNullOrWhiteSpace(createTransferRequestDto.Description)
                ? new TransferDescription(createTransferRequestDto.Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(createTransferRequestDto.Reference)
                ? new TransferReference(createTransferRequestDto.Reference)
                : null,
            CreatedBy = new Username(username)
        };
    }
}