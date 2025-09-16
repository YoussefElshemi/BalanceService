using Core.ValueObjects;

namespace Core.Models;

public record InterestAccrual : DeletableBaseModel
{
    public required InterestAccrualId InterestAccrualId { get; init; }
    public required AccountId AccountId { get; init; }
    public required InterestProductId InterestProductId { get; init; }
    public required InterestRate DailyInterestRate { get; init; }
    public required AccruedAt AccruedAt { get; init; }
    public required AccruedAmount AccruedAmount { get; init; }
    public required bool IsPosted { get; init; }
    public required PostedAt? PostedAt { get; init; }
}
