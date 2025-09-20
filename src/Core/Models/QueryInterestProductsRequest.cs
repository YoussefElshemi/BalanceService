using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record QueryInterestProductsRequest
{
    public required PageSize PageSize { get; init; }
    public required PageNumber PageNumber { get; init; }
    public required InterestProductName? Name { get; init; }
    public required InterestPayoutFrequency? InterestPayoutFrequency { get; init; }
}