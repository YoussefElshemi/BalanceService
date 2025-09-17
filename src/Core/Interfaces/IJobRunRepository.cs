using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IJobRunRepository
{
    Task CreateAsync(JobRun jobRun, CancellationToken cancellationToken);
    Task ExecuteAsync(JobRunId jobRunId, Username executedBy, CancellationToken cancellationToken);
    Task DeleteAsync(JobRunId jobRunId, Username deletedBy, CancellationToken cancellationToken);
}