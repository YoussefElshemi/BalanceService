using Core.Enums;
using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class CreateHoldRequestDtoMapper
{
    public static CreateHoldRequest ToModel(this CreateHoldRequestDto createHoldRequestDto, string username)
    {
        return new CreateHoldRequest
        {
            AccountId = new AccountId(createHoldRequestDto.AccountId),
            Amount = new HoldAmount(createHoldRequestDto.Amount),
            CurrencyCode = createHoldRequestDto.CurrencyCode,
            IdempotencyKey = new IdempotencyKey(createHoldRequestDto.IdempotencyKey),
            Status = HoldStatus.Active,
            ExpiresAt = createHoldRequestDto.ExpiresAt.HasValue
                ? new  ExpiresAt(createHoldRequestDto.ExpiresAt.Value)
                : null,
            Type = createHoldRequestDto.Type,
            Source = HoldSource.Api,
            Description = !string.IsNullOrWhiteSpace(createHoldRequestDto.Description)
                ? new HoldDescription(createHoldRequestDto.Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(createHoldRequestDto.Reference)
                ? new HoldReference(createHoldRequestDto.Reference)
                : null,
            CreatedBy = new Username(username)
        };
    }
}