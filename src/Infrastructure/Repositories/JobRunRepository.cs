using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class JobRunRepository(
    ApplicationDbContext dbContext,
    TimeProvider timeProvider) : IJobRunRepository
{
    public async Task CreateAsync(JobRun jobRun, CancellationToken cancellationToken)
    {
        var jobRunEntity = jobRun.ToEntity();
        await dbContext.JobRuns.AddAsync(jobRunEntity, cancellationToken);
    }

    public async Task ExecuteAsync(JobRunId jobRunId, Username executedBy, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var jobRunEntity = await dbContext.JobRuns
            .AsTracking()
            .FirstAsync(x => x.JobRunId == jobRunId,  cancellationToken);

        jobRunEntity.IsExecuted = true;
        jobRunEntity.ExecutedAt = utcDateTime;
        jobRunEntity.UpdatedBy = executedBy;
        jobRunEntity.UpdatedAt = utcDateTime;
    }

    public async Task DeleteAsync(JobRunId jobRunId, Username deletedBy, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var jobRunEntity = await dbContext.JobRuns
            .AsTracking()
            .FirstAsync(x => x.JobRunId == jobRunId,  cancellationToken);

        jobRunEntity.IsDeleted = false;
        jobRunEntity.DeletedBy = deletedBy;
        jobRunEntity.DeletedAt = utcDateTime;
    }
}