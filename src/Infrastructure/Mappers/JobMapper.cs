using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;

namespace Infrastructure.Mappers;

public static class JobMapper
{
    public static JobEntity ToEntity(this Job job)
    {
        return new JobEntity
        {
            JobId = job.JobId,
            JobName = job.JobName,
            CreatedAt = job.CreatedAt,
            CreatedBy = job.CreatedBy,
            UpdatedAt = job.UpdatedAt,
            UpdatedBy = job.UpdatedBy,
            IsDeleted = job.IsDeleted,
            DeletedAt = job.DeletedAt,
            DeletedBy = job.DeletedBy
        };
    }
    
    public static Job ToModel(this JobEntity jobEntity)
    {
        return new Job
        {
            JobId = new JobId(jobEntity.JobId),
            JobName = new JobName(jobEntity.JobName),
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