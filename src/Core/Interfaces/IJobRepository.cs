using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IJobRepository
{
    Task CreateAsync(Job job, CancellationToken cancellationToken);
    Task<Job?> GetByNameAsync(JobName jobName, CancellationToken cancellationToken);
}