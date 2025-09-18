using Core.ValueObjects;

namespace Core.Models;

public record UpdateInterestProductAccountLinkActiveRequest
{
    public required AccountId AccountId { get; init; }
    public required InterestProductId InterestProductId { get; init; }
    public required bool IsActive { get; init; }
    public required Username UpdatedBy { get; init; }
}