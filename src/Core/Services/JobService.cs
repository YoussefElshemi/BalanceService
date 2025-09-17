using Core.Configs;
using Core.Constants;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Core.Services;

public class JobService(
    IJobRepository jobRepository,
    IJobRunRepository jobRunRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IJobService
{
    public async Task<Job> EnsureJobExistsAsync(IJobConfig jobConfig, CancellationToken cancellationToken)
    {
        var existingJob = await jobRepository.GetByNameAsync(new JobName(jobConfig.JobName), cancellationToken);
        if (existingJob != null)
        {
            return existingJob;
        }

        var job = new Job
        {
            JobId = new JobId(Guid.NewGuid()),
            JobName = new JobName(jobConfig.JobName),
            CreatedAt = new CreatedAt(timeProvider.GetUtcNow()),
            CreatedBy = SystemConstants.Username,
            UpdatedAt = new UpdatedAt(timeProvider.GetUtcNow()),
            UpdatedBy = SystemConstants.Username,
            IsDeleted = false,
            DeletedAt = null,
            DeletedBy = null,
        };
            
        await jobRepository.CreateAsync(job, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return job;
    }

    public async Task<(bool Success, JobRun JobRun)> TryCreateRunAsync(JobId jobId, ScheduledAt scheduledAt, CancellationToken cancellationToken)
    {
        var jobRun = new JobRun
        {
            JobRunId = new JobRunId(Guid.NewGuid()),
            JobId = jobId,
            ScheduledAt = scheduledAt,
            IsExecuted = false,
            ExecutedAt = null,
            CreatedAt = new CreatedAt(timeProvider.GetUtcNow()),
            CreatedBy = SystemConstants.Username,
            UpdatedAt = new UpdatedAt(timeProvider.GetUtcNow()),
            UpdatedBy = SystemConstants.Username,
            IsDeleted = false,
            DeletedAt = null,
            DeletedBy = null
        };

        try
        {
            await jobRunRepository.CreateAsync(jobRun, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return (true, jobRun);
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                return (false, jobRun);
            }

            throw;
        }
    }

    public async Task ExecuteAsync(JobRunId jobRunId, Username executedBy, CancellationToken cancellationToken)
    {
        await jobRunRepository.ExecuteAsync(jobRunId, executedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(JobRunId jobRunId, Username deletedBy, CancellationToken cancellationToken)
    {
        await jobRunRepository.DeleteAsync(jobRunId, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}