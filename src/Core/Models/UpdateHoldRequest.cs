using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record UpdateHoldRequest
{
    public required HoldType Type { get; init; }
    public required ExpiresAt? ExpiresAt { get; init; }
    public required HoldDescription? Description { get; init; }
    public required HoldReference? Reference { get; init; }
    public required Username UpdatedBy { get; init; }
}