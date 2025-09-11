namespace Infrastructure.Entities;

public record AccountTypeEntity
{
    public required int AccountTypeId { get; init; }
    public required string Name { get; init; }
    public ICollection<AccountEntity> AccountEntities { get; init; } = null!;
}