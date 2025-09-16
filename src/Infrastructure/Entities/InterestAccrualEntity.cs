namespace Infrastructure.Entities;

public record InterestAccrualEntity : DeletableBaseEntity
{
    public required Guid InterestAccrualId { get; init; }
    public required Guid AccountId { get; init; }
    public required Guid InterestProductId { get; init; }
    public required decimal DailyInterestRate { get; init; }
    public required DateTimeOffset AccruedAt { get; init; }
    public required decimal AccruedAmount { get; init; }
    public required bool IsPosted { get; set; }
    public required DateTimeOffset? PostedAt { get; set; }
    
    public AccountEntity AccountEntity { get; init; } = null!;
    public InterestProductEntity InterestProductEntity { get; init; } = null!;
}
