using Core.ValueObjects;

namespace Core.Models;

public record JobRun : DeletableBaseModel
{
    public required JobRunId JobRunId { get; init; }
    public required JobId JobId { get; init; }
    public required ScheduledAt ScheduledAt { get; init; }
    public required bool IsExecuted { get; init; }
    public required ExecutedAt? ExecutedAt { get; init; }
}