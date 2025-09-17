using Core.Configs;
using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IJobService
{
    Task<Job> GetOrCreateAsync(IJobConfig jobConfig, CancellationToken cancellationToken);
    Task<(bool Success, JobRun JobRun)> TryCreateRunAsync(JobId jobId, ScheduledAt scheduledAt, CancellationToken cancellationToken);
    Task ExecuteAsync(JobRunId jobRunId, Username executedBy, CancellationToken cancellationToken);
    Task DeleteAsync(JobRunId jobRunId, Username deletedBy, CancellationToken cancellationToken);
}