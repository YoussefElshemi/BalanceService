namespace Infrastructure.Entities;

public record InterestPayoutFrequencyEntity
{
    public required int InterestPayoutFrequencyId { get; init; }
    public required string Name { get; init; }
    public ICollection<InterestProductEntity> InterestProductEntities { get; init; } = null!;
}
