using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Entities.History;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class HistoryRepository<TEntity, TModel>(
    ApplicationDbContext dbContext,
    TimeProvider timeProvider) : IHistoryRepository<TEntity, TModel>
    where TEntity : class, IHistoryEntity<TModel>
{

    public async Task<List<TModel>> GetPendingAsync(int count, CancellationToken cancellationToken)
    {
        var entities = await dbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(x => x.ProcessingStatusId == (int)ProcessingStatus.NotProcessed)
            .OrderBy(x => x.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);

        return entities.Select(x => x.ToModel()).ToList();
    }

    public async Task UpdateProcessingStatusAsync(Guid primaryKey, ProcessingStatus processingStatus, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Set<TEntity>().FindAsync([primaryKey], cancellationToken)
                     ?? throw new NotFoundException();

        entity.ProcessingStatusId = (int)processingStatus;

        dbContext.Attach(entity);
        dbContext.Entry(entity).Property(e => e.ProcessingStatusId).IsModified = true;
    }

    public async Task MarkAsProcessedAsync(Guid primaryKey, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var entity = await dbContext.Set<TEntity>().FindAsync([primaryKey], cancellationToken)
                     ?? throw new NotFoundException();

        entity.ProcessingStatusId = (int)ProcessingStatus.Processed;
        entity.ProcessedAt = utcDateTime;

        dbContext.Attach(entity);
        dbContext.Entry(entity).Property(e => e.ProcessingStatusId).IsModified = true;
        dbContext.Entry(entity).Property(e => e.ProcessedAt).IsModified = true;
    }
}