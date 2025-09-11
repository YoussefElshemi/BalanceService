namespace Infrastructure.Entities;

public record HoldStatusEntity
{
    public required int HoldStatusId { get; init; }
    public required string Name { get; init; }
    public ICollection<HoldEntity> HoldEntities { get; init; } = null!;
}
