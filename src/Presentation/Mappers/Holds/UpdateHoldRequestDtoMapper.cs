using Core.Models;
using Core.ValueObjects;
using Presentation.Models.Holds;

namespace Presentation.Mappers.Holds;

public static class UpdateHoldRequestDtoMapper
{
    public static UpdateHoldRequest ToModel(this UpdateHoldRequestDto updateHoldRequestDto)
    {
        return new UpdateHoldRequest
        {
            HoldId = new HoldId(updateHoldRequestDto.HoldId),
            ExpiresAt = updateHoldRequestDto.ExpiresAt.HasValue
                ? new  ExpiresAt(updateHoldRequestDto.ExpiresAt.Value)
                : null,
            Type = updateHoldRequestDto.Type,
            Description = !string.IsNullOrWhiteSpace(updateHoldRequestDto.Description)
                ? new HoldDescription(updateHoldRequestDto.Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(updateHoldRequestDto.Reference)
                ? new HoldReference(updateHoldRequestDto.Reference)
                : null,
            UpdatedBy = new Username(updateHoldRequestDto.Username)
        };
    }
}