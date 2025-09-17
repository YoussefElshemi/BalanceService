namespace Infrastructure.Entities;

public record JobEntity : DeletableBaseEntity
{
    public required Guid JobId { get; init; }
    public required string JobName { get; init; }

    public ICollection<JobRunEntity> JobRunEntities { get; init; } = null!;
}
