using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class UpdateHoldRequestDtoMapper
{
    public static UpdateHoldRequest ToModel(this UpdateHoldRequestDto createHoldRequestDto, string username)
    {
        return new UpdateHoldRequest
        {
            ExpiresAt = createHoldRequestDto.ExpiresAt.HasValue
                ? new  ExpiresAt(createHoldRequestDto.ExpiresAt.Value)
                : null,
            Type = createHoldRequestDto.Type,
            Description = !string.IsNullOrWhiteSpace(createHoldRequestDto.Description)
                ? new HoldDescription(createHoldRequestDto.Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(createHoldRequestDto.Reference)
                ? new HoldReference(createHoldRequestDto.Reference)
                : null,
            UpdatedBy = new Username(username)
        };
    }
}