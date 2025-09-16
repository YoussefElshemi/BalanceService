namespace Infrastructure.Entities;

public record InterestProductAccountLinkEntity : DeletableBaseEntity
{
    public required Guid InterestProductId { get; init; }
    public required Guid AccountId { get; init; }
    public required bool IsActive {  get; init; }

    public AccountEntity AccountEntity { get; init; } = null!;
    public InterestProductEntity InterestProductEntity { get; init; } = null!;
}
