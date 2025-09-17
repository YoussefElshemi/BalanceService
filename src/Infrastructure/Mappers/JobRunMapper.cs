using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;

namespace Infrastructure.Mappers;

public static class JobRunMapper
{
    public static JobRunEntity ToEntity(this JobRun job)
    {
        return new JobRunEntity
        {
            JobRunId = job.JobRunId,
            JobId = job.JobId,
            ScheduledAt = job.ScheduledAt,
            IsExecuted = job.IsExecuted,
            ExecutedAt = job.ExecutedAt,
            CreatedAt = job.CreatedAt,
            CreatedBy = job.CreatedBy,
            UpdatedAt = job.UpdatedAt,
            UpdatedBy = job.UpdatedBy,
            IsDeleted = job.IsDeleted,
            DeletedAt = job.DeletedAt,
            DeletedBy = job.DeletedBy
        };
    }
    
    public static JobRun ToModel(this JobRunEntity jobEntity)
    {
        return new JobRun
        {
            JobRunId = new JobRunId(jobEntity.JobRunId),
            JobId = new JobId(jobEntity.JobId),
            ScheduledAt = new ScheduledAt(jobEntity.ScheduledAt),
            IsExecuted = jobEntity.IsExecuted,
            ExecutedAt = jobEntity.ExecutedAt.HasValue
                ? new ExecutedAt(jobEntity.ExecutedAt.Value)
                : null,
            CreatedAt = new CreatedAt(jobEntity.CreatedAt),
            CreatedBy = new Username(jobEntity.CreatedBy),
            UpdatedAt = new UpdatedAt(jobEntity.UpdatedAt),
            UpdatedBy = new Username(jobEntity.UpdatedBy),
            IsDeleted = jobEntity.IsDeleted,
            DeletedAt = jobEntity.DeletedAt.HasValue
                ? new DeletedAt(jobEntity.DeletedAt.Value)
                : null,
            DeletedBy = !string.IsNullOrWhiteSpace(jobEntity.DeletedBy)
                ? new Username(jobEntity.DeletedBy)
                : null
        };
    }
}