using Core.Enums;

namespace Presentation.Models;

public record UpdateHoldRequestDto
{
    public required HoldType Type { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
    public string? Description { get; init; }
    public string? Reference { get; init; }
}