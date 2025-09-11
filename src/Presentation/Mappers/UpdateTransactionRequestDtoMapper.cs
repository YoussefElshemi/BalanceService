using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class UpdateTransactionRequestDtoMapper
{
    public static UpdateTransactionRequest ToModel(this UpdateTransactionRequestDto updateTransactionRequestDto, string username)
    {
        return new UpdateTransactionRequest
        {
            Type = updateTransactionRequestDto.Type,
            Source = updateTransactionRequestDto.Source,
            Description = !string.IsNullOrWhiteSpace(updateTransactionRequestDto.Description)
                ? new TransactionDescription(updateTransactionRequestDto.Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(updateTransactionRequestDto.Reference)
                ? new TransactionReference(updateTransactionRequestDto.Reference)
                : null,
            UpdatedBy = new Username(username)
        };
    }
}