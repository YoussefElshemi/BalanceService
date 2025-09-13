using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Entities;
using Infrastructure.Entities.History;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class HistoryRepository<TEntity, TModel, TKey>(
    ApplicationDbContext dbContext,
    TimeProvider timeProvider) : IHistoryRepository<TEntity, TModel, TKey>
    where TEntity : class, IHistoryEntity<TModel>
{

    public async Task<List<TModel>> GetPendingAsync(int count, CancellationToken cancellationToken)
    {
        var entities = await dbContext.Set<TEntity>()
            .AsNoTracking()
            .Where(x => x.IsProcessed == false)
            .OrderBy(x => x.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);

        return entities.Select(x => x.ToModel()).ToList();
    }

    public async Task MarkAsProcessedAsync(TKey primaryKey, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var entity = await dbContext.Set<TEntity>().FindAsync([primaryKey], cancellationToken)
                     ?? throw new NotFoundException();

        entity.IsProcessed = true;
        entity.ProcessedAt = utcDateTime;

        dbContext.Attach(entity);
        dbContext.Entry(entity).Property(e => e.IsProcessed).IsModified = true;
        dbContext.Entry(entity).Property(e => e.ProcessedAt).IsModified = true;
    }
}