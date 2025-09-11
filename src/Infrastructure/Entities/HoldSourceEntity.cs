namespace Infrastructure.Entities;

public record HoldSourceEntity
{
    public required int HoldSourceId { get; init; }
    public required string Name { get; init; }
    public ICollection<HoldEntity> HoldEntities { get; init; } = null!;
}
