using Core.Models;
using Core.ValueObjects;
using Presentation.Models.Transactions;

namespace Presentation.Mappers.Transactions;

public static class UpdateTransactionRequestDtoMapper
{
    public static UpdateTransactionRequest ToModel(this UpdateTransactionRequestDto updateTransactionRequestDto)
    {
        return new UpdateTransactionRequest
        {
            TransactionId = new TransactionId(updateTransactionRequestDto.TransactionId),
            Type = updateTransactionRequestDto.Type,
            Source = updateTransactionRequestDto.Source,
            Description = !string.IsNullOrWhiteSpace(updateTransactionRequestDto.Description)
                ? new TransactionDescription(updateTransactionRequestDto.Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(updateTransactionRequestDto.Reference)
                ? new TransactionReference(updateTransactionRequestDto.Reference)
                : null,
            UpdatedBy = new Username(updateTransactionRequestDto.Username)
        };
    }
}