namespace Infrastructure.Entities;

public record InterestProductEntity : DeletableBaseEntity
{
    public required Guid InterestProductId { get; init; }
    public required string Name { get; set; }
    public required decimal AnnualInterestRate { get; set; }
    public required int InterestPayoutFrequencyId { get; set; }
    public required int AccrualBasis { get; set; }

    public InterestPayoutFrequencyEntity InterestPayoutFrequencyEntity { get; init; } = null!;
    public ICollection<InterestProductAccountLinkEntity>? InterestProductAccountLinkEntities { get; init; }
    public ICollection<InterestAccrualEntity>? InterestAccrualEntities { get; init; }
}
