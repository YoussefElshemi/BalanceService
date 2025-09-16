namespace Infrastructure.Entities;

public record InterestProductEntity : DeletableBaseEntity
{
    public required Guid InterestProductId { get; init; }
    public required string Name { get; init; }
    public required decimal AnnualInterestRate { get; init; }
    public required int InterestPayoutFrequencyId { get; init; }
    public required int AccrualBasis { get; init; }

    public InterestPayoutFrequencyEntity InterestPayoutFrequencyEntity { get; init; } = null!;
    public ICollection<InterestProductAccountLinkEntity>? InterestProductAccountLinkEntities { get; init; }
    public ICollection<InterestAccrualEntity>? InterestAccrualEntities { get; init; }
}
