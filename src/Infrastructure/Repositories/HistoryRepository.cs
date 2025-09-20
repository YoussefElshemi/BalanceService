// ReSharper disable EntityFramework.ClientSideDbFunctionCall

using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Entities.History;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class HistoryRepository<TEntity, TModel>(
    ApplicationDbContext dbContext,
    TimeProvider timeProvider) : IHistoryRepository<TEntity, TModel>
    where TEntity : class, IHistoryEntity<TModel>
{
    public Task<int> CountChangesAsync(GetHistoryRequest getHistoryRequest, CancellationToken cancellationToken)
    {
        var query = BuildSearchQuery(getHistoryRequest);

        return query.CountAsync(cancellationToken);
    }

    public async Task<List<ChangeEvent>> GetChangesAsync(GetHistoryRequest getHistoryRequest, CancellationToken cancellationToken)
    {
        var query = BuildSearchQuery(getHistoryRequest);

        var entities = await query
            .OrderBy(x => x.Timestamp)
            .ThenBy(x => x.Field)
            .Skip((getHistoryRequest.PageNumber - 1) * getHistoryRequest.PageSize)
            .Take(getHistoryRequest.PageSize)
            .ToListAsync(cancellationToken);

        return entities.Select(x => x.ToModel()).ToList();
    }

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

    private IQueryable<ChangeEventEntity> BuildSearchQuery(GetHistoryRequest getHistoryRequest)
    {
        var tableName = dbContext.Model.FindEntityType(typeof(TEntity))!.GetTableName();
        var idColumn = TEntity.GetIdColumn();
        var properties = TEntity.GetColumns();

        var unionSql = string.Join("\nUNION ALL\n", properties.Select(col => $@"
        SELECT 
            ""{idColumn}"" AS ""{nameof(ChangeEventEntity.EntityId)}"",
            ""{nameof(ChangeEventEntity.Timestamp)}"",
            '{col}' AS ""{nameof(ChangeEventEntity.Field)}"",
            LAG(""{col}"") OVER (PARTITION BY ""{idColumn}"" ORDER BY ""{nameof(ChangeEventEntity.Timestamp)}"")::text AS ""{nameof(ChangeEventEntity.OldValue)}"",
            ""{col}""::text AS ""{nameof(ChangeEventEntity.NewValue)}"",
            ""{nameof(ChangeEventEntity.HistoryTypeId)}"",
            ROW_NUMBER() OVER (PARTITION BY ""{idColumn}"" ORDER BY ""{nameof(ChangeEventEntity.Timestamp)}"") AS row_num
        FROM ""{tableName}""
        WHERE ""{idColumn}"" = '{getHistoryRequest.EntityId}'
    "));

        var sql = $@"
        WITH Changes AS (
           {unionSql}
        )
        SELECT *
        FROM Changes 
        WHERE ""{nameof(ChangeEventEntity.OldValue)}"" IS DISTINCT FROM ""{nameof(ChangeEventEntity.NewValue)}""";

        if (getHistoryRequest.IgnoreInsert == true)
        {
            sql += $@"
          AND NOT (row_num = 1 AND ""{nameof(ChangeEventEntity.HistoryTypeId)}"" = {(int)HistoryType.Insert})";
        }

        return dbContext.Database.SqlQueryRaw<ChangeEventEntity>(sql);
    }

}