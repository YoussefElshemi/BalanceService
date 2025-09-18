using Core.ValueObjects;

namespace Core.Models;

public record InterestProductAccountLink : DeletableBaseModel
{
    public required InterestProduct InterestProduct { get; init; }
    public required Account Account { get; init; }
    public required bool IsActive { get; init; }
    public required ExpiresAt? ExpiresAt { get; init; } 
}
