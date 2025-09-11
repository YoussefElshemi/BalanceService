namespace Infrastructure.Entities;

public record AccountStatusEntity
{
    public required int AccountStatusId { get; init; }
    public required string Name { get; init; }
    public ICollection<AccountEntity> AccountEntities { get; init; } = null!;
}
