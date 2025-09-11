namespace Infrastructure.Entities;

public record HoldTypeEntity
{
    public required int HoldTypeId { get; init; }
    public required string Name { get; init; }
    public ICollection<HoldEntity> HoldEntities { get; init; } = null!;
}
