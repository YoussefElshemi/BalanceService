using Core.Enums;

namespace Presentation.Models;

public record UpdateInterestProductRequestDto
{
    public required string Name { get; init; }
    public required decimal AnnualInterestRate { get; init; }
    public required InterestPayoutFrequency InterestPayoutFrequency { get; init; }
    public required int AccrualBasis { get; init; }
}