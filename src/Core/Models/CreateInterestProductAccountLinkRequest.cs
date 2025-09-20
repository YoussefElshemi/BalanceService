using Core.ValueObjects;

namespace Core.Models;

public record CreateInterestProductAccountLinkRequest
{
    public required AccountId AccountId { get; init; }
    public required InterestProductId InterestProductId { get; init; }
    public required ExpiresAt? ExpiresAt { get; init; }
    public required Username CreatedBy { get; init; }
}