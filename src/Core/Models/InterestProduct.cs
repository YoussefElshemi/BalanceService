using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record InterestProduct : DeletableBaseModel
{
    public required InterestProductId InterestProductId { get; init; }
    public required InterestProductName Name { get; init; }
    public required InterestRate AnnualInterestRate { get; init; }
    public required InterestPayoutFrequency InterestPayoutFrequency { get; init; }
    public required AccrualBasis AccrualBasis { get; init; }
}
