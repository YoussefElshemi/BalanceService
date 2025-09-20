using Core.Constants;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Extensions;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class HoldRepository(
    ApplicationDbContext dbContext,
    TimeProvider timeProvider) : IHoldRepository
{
    public async Task CreateAsync(Hold hold, CancellationToken cancellationToken)
    {
        var holdEntity = hold.ToEntity();
        await dbContext.Holds.AddAsync(holdEntity, cancellationToken);
    }

    public async Task<Hold?> GetByIdAsync(HoldId holdId, CancellationToken cancellationToken)
    {
        var holdEntity = await dbContext.Holds
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.HoldId == holdId && x.IsDeleted == false, cancellationToken);

        return holdEntity?.ToModel();
    }

    public Task<bool> ExistsAsync(HoldId holdId, CancellationToken cancellationToken)
    {
        return dbContext.Holds
            .AsNoTracking()
            .AnyAsync(x => x.HoldId ==  holdId && x.IsDeleted == false, cancellationToken);
    }

    public async Task ReleaseAsync(HoldId holdId, Username releasedBy, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var holdEntity = await dbContext.Holds
            .AsTracking()
            .FirstAsync(x => x.HoldId == holdId, cancellationToken);

        holdEntity.HoldStatusId = (int)HoldStatus.Released;
        holdEntity.UpdatedBy = releasedBy;
        holdEntity.UpdatedAt = utcDateTime;
    }

    public async Task DeleteAsync(HoldId holdId, Username deletedBy, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var holdEntity = await dbContext.Holds
            .AsTracking()
            .FirstAsync(x => x.HoldId == holdId, cancellationToken);

        holdEntity.IsDeleted = true;
        holdEntity.DeletedBy = deletedBy;
        holdEntity.DeletedAt = utcDateTime;
    }

    public async Task SettleAsync(HoldId holdId, TransactionId transactionId, Username settledBy, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var holdEntity = await dbContext.Holds
            .AsTracking()
            .FirstAsync(x => x.HoldId == holdId, cancellationToken);

        holdEntity.HoldStatusId = (int)HoldStatus.Settled;
        holdEntity.SettledTransactionId = transactionId;
        holdEntity.UpdatedBy = settledBy;
        holdEntity.UpdatedAt = utcDateTime;
    }

    public async Task<Hold> UpdateAsync(UpdateHoldRequest updateHoldRequest, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var holdEntity = await dbContext.Holds
            .AsTracking()
            .FirstAsync(x => x.HoldId == updateHoldRequest.HoldId, cancellationToken);

        holdEntity.HoldTypeId = (int)updateHoldRequest.Type;
        holdEntity.ExpiresAt =  updateHoldRequest.ExpiresAt;
        holdEntity.Description =  updateHoldRequest.Description;
        holdEntity.Reference = updateHoldRequest.Reference;
        holdEntity.UpdatedBy = updateHoldRequest.UpdatedBy;
        holdEntity.UpdatedAt = utcDateTime;

        return holdEntity.ToModel();
    }

    public Task<int> CountAsync(QueryHoldsRequest queryHoldsRequest, CancellationToken cancellationToken)
    {
        var query = dbContext.Holds.BuildSearchQuery(queryHoldsRequest);

        return query.CountAsync(cancellationToken);
    }

    public async Task<List<Hold>> QueryAsync(QueryHoldsRequest queryHoldsRequest, CancellationToken cancellationToken)
    {
        var query = dbContext.Holds.BuildSearchQuery(queryHoldsRequest);

        var entities = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.HoldId)
            .Skip((queryHoldsRequest.PageNumber - 1) * queryHoldsRequest.PageSize)
            .Take(queryHoldsRequest.PageSize)
            .ToListAsync(cancellationToken);

        return entities.Select(x => x.ToModel()).ToList();
    }

    public async Task ExpireHoldsAsync(CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var holdEntities = await dbContext.Holds
            .AsTracking()
            .Where(q => q.HoldStatusId == (int)HoldStatus.Active && q.ExpiresAt <= timeProvider.GetUtcNow())
            .ToListAsync(cancellationToken);

        if (holdEntities.Count != 0)
        {
            foreach (var holdEntity in holdEntities)
            {
                holdEntity.HoldStatusId = (int)HoldStatus.Expired;
                holdEntity.UpdatedBy = SystemConstants.Username;
                holdEntity.UpdatedAt = utcDateTime;
            }
        }
    }
}