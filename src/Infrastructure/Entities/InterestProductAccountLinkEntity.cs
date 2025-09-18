namespace Infrastructure.Entities;

public record InterestProductAccountLinkEntity : DeletableBaseEntity
{
    public required Guid InterestProductId { get; init; }
    public required Guid AccountId { get; init; }
    public required bool IsActive {  get; set; }
    public required DateTimeOffset? ExpiresAt {  get; set; }

    public AccountEntity AccountEntity { get; init; } = null!;
    public InterestProductEntity InterestProductEntity { get; init; } = null!;
}
