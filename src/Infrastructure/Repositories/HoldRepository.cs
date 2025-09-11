using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Constants;
using Infrastructure.Entities;
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

    public async Task<Hold> UpdateAsync(HoldId holdId, UpdateHoldRequest updateHoldRequest, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var holdEntity = await dbContext.Holds
            .AsTracking()
            .FirstAsync(x => x.HoldId == holdId, cancellationToken);

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
        var query = BuildSearchQuery(queryHoldsRequest);

        return query.CountAsync(cancellationToken);
    }

    public async Task<List<Hold>> QueryAsync(QueryHoldsRequest queryHoldsRequest, CancellationToken cancellationToken)
    {
        var query = BuildSearchQuery(queryHoldsRequest);

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

    private IQueryable<HoldEntity> BuildSearchQuery(QueryHoldsRequest queryHoldsRequest)
    {
        var query = dbContext.Holds
            .AsNoTracking()
            .Where(x => x.IsDeleted == false);

        if (queryHoldsRequest.AccountId.HasValue)
        {
            query = query.Where(x => x.AccountId == queryHoldsRequest.AccountId);
        }

        if (queryHoldsRequest.CurrencyCode.HasValue)
        {
            query = query.Where(x => x.CurrencyCode == queryHoldsRequest.CurrencyCode.ToString());
        }

        if (queryHoldsRequest.Amount.HasValue)
        {
            query = query.Where(x => x.Amount == queryHoldsRequest.Amount);
        }

        if (queryHoldsRequest.SettledTransactionId.HasValue)
        {
            query = query.Where(x => x.SettledTransactionId == queryHoldsRequest.SettledTransactionId);
        }

        if (queryHoldsRequest.ExpiresAt.HasValue)
        {
            query = query.Where(x => x.ExpiresAt == queryHoldsRequest.ExpiresAt);
        }

        if (queryHoldsRequest.Type.HasValue)
        {
            query = query.Where(x => x.HoldTypeId == (int)queryHoldsRequest.Type);
        }

        if (queryHoldsRequest.Status.HasValue)
        {
            query = query.Where(x => x.HoldStatusId == (int)queryHoldsRequest.Status);
        }

        if (queryHoldsRequest.Source.HasValue)
        {
            query = query.Where(x => x.HoldSourceId == (int)queryHoldsRequest.Source);
        }

        if (queryHoldsRequest.Description.HasValue)
        {
            query = query.Where(x => x.Description != null && EF.Functions.ILike(x.Description, $"%{queryHoldsRequest.Description}%"));
        }

        if (queryHoldsRequest.Reference.HasValue)
        {
            query = query.Where(x => x.Reference != null && EF.Functions.ILike(x.Reference, $"%{queryHoldsRequest.Reference}%"));
        }

        return query;
    }
}