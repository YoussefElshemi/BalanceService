namespace Infrastructure.Entities;

public record JobRunEntity : DeletableBaseEntity
{
    public required Guid JobRunId { get; init; }
    public required Guid JobId { get; init; }
    public required DateTimeOffset ScheduledAt { get; init; }
    public required bool IsExecuted { get; set; }
    public required DateTimeOffset? ExecutedAt { get; set; }

    public JobEntity JobEntity { get; init; } = null!;
}
