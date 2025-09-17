using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class JobRepository(ApplicationDbContext dbContext) : IJobRepository
{
    public async Task CreateAsync(Job job, CancellationToken cancellationToken)
    {
        var jobEntity = job.ToEntity();
        await dbContext.Jobs.AddAsync(jobEntity, cancellationToken);
    }

    public async Task<Job?> GetByNameAsync(JobName jobName, CancellationToken cancellationToken)
    {
        var jobEntity = await dbContext.Jobs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.JobName == jobName, cancellationToken);

        return jobEntity?.ToModel();
    }
}