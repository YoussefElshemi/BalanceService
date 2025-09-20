using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Core.ValueObjects;
using Infrastructure.Extensions;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InterestProductAccountLinkRepository(
    ApplicationDbContext dbContext,
    TimeProvider timeProvider) : IInterestProductAccountLinkRepository
{
    public async Task CreateAsync(InterestProductAccountLink interestProductAccountLink, CancellationToken cancellationToken)
    {
        var interestProductAccountLinkEntity = interestProductAccountLink.ToEntity();
        await dbContext.InterestProductAccountLinks.AddAsync(interestProductAccountLinkEntity, cancellationToken);
    }

    public async Task<InterestProductAccountLink?> GetByIdAsync(AccountId accountId, InterestProductId interestProductId, CancellationToken cancellationToken)
    {
        var interestProductAccountLinkEntity = await dbContext.InterestProductAccountLinks
            .AsNoTracking()
            .Include(x => x.AccountEntity)
            .Include(x => x.InterestProductEntity)
            .FirstOrDefaultAsync(x => x.AccountId == accountId &&
                                      x.InterestProductId == interestProductId
                                      && x.IsDeleted == false, cancellationToken);

        return interestProductAccountLinkEntity?.ToModel();
    }

    public async Task ActivateAsync(
        AccountId accountId,
        InterestProductId interestProductId,
        Username activatedBy,
        CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var interestProductAccountLinkEntity = await dbContext.InterestProductAccountLinks
            .AsTracking()
            .FirstAsync(x => x.AccountId == accountId && x.InterestProductId == interestProductId, cancellationToken);

        interestProductAccountLinkEntity.IsActive = true;
        interestProductAccountLinkEntity.UpdatedBy = activatedBy;
        interestProductAccountLinkEntity.UpdatedAt = utcDateTime;
    }

    public async Task DeactivateAsync(
        AccountId accountId,
        InterestProductId interestProductId,
        Username deactivatedBy,
        CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var interestProductAccountLinkEntity = await dbContext.InterestProductAccountLinks
            .AsTracking()
            .FirstAsync(x => x.AccountId == accountId && x.InterestProductId == interestProductId, cancellationToken);

        interestProductAccountLinkEntity.IsActive = false;
        interestProductAccountLinkEntity.UpdatedBy = deactivatedBy;
        interestProductAccountLinkEntity.UpdatedAt = utcDateTime;
    }

    public async Task<List<InterestProductAccountLink>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var entities = await dbContext.InterestProductAccountLinks
            .AsNoTracking()
            .Where(x => x.IsActive == true && x.IsDeleted == false)
            .Where(x => x.ExpiresAt == null || x.ExpiresAt <= timeProvider.GetUtcNow())
            .Include(x => x.AccountEntity)
            .Include(x => x.InterestProductEntity)
            .ToListAsync(cancellationToken);

        entities = entities
            .Where(x =>
                AccountRulesService.IsAllowed(
                    (AccountStatus)x.AccountEntity.AccountStatusId,
                    AccountOperationType.AccrueInterest))
            .ToList();

        return entities.Select(x => x.ToModel()).ToList();
    }

    public Task<bool> ExistsAsync(AccountId accountId, InterestProductId interestProductId, CancellationToken cancellationToken)
    {
        return dbContext.InterestProductAccountLinks
            .AsNoTracking()
            .AnyAsync(x => x.AccountId == accountId && x.InterestProductId == interestProductId, cancellationToken);
    }

    public async Task<InterestProductAccountLink> UpdateAsync(
        UpdateInterestProductAccountLinkRequest updateInterestProductAccountLinkRequest,
        CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var interestProductAccountLinkEntity = await dbContext.InterestProductAccountLinks
            .AsTracking()
            .FirstAsync(x => x.AccountId == updateInterestProductAccountLinkRequest.AccountId &&
                             x.InterestProductId == updateInterestProductAccountLinkRequest.InterestProductId, cancellationToken);

        interestProductAccountLinkEntity.UpdatedBy = updateInterestProductAccountLinkRequest.UpdatedBy;
        interestProductAccountLinkEntity.UpdatedAt = utcDateTime;
        
        return interestProductAccountLinkEntity.ToModel();
    }

    public async Task DeleteAsync(
        AccountId accountId,
        InterestProductId interestProductId,
        Username deletedBy,
        CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var interestProductAccountLinkEntity = await dbContext.InterestProductAccountLinks
            .AsTracking()
            .FirstAsync(x => x.AccountId == accountId && x.InterestProductId == interestProductId, cancellationToken);

        interestProductAccountLinkEntity.IsDeleted = true;
        interestProductAccountLinkEntity.DeletedBy = deletedBy;
        interestProductAccountLinkEntity.DeletedAt = utcDateTime;
    }

    public Task<int> CountAsync(
        QueryInterestProductAccountLinksRequest queryInterestProductAccountLinksRequest,
        CancellationToken cancellationToken)
    {
        var query = dbContext.InterestProductAccountLinks.BuildSearchQuery(queryInterestProductAccountLinksRequest);

        return query.CountAsync(cancellationToken);
    }

    public async Task<List<InterestProductAccountLink>> QueryAsync(
        QueryInterestProductAccountLinksRequest queryInterestProductAccountLinksRequest,
        CancellationToken cancellationToken)
    {
        var query = dbContext.InterestProductAccountLinks.BuildSearchQuery(queryInterestProductAccountLinksRequest);
        
        var entities = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.InterestProductId)
            .ThenByDescending(x => x.AccountId)
            .Skip((queryInterestProductAccountLinksRequest.PageNumber - 1) * queryInterestProductAccountLinksRequest.PageSize)
            .Take(queryInterestProductAccountLinksRequest.PageSize)
            .ToListAsync(cancellationToken);

        return entities.Select(x => x.ToModel()).ToList();
    }

    public async Task MarkAsInactiveAsync(InterestProductId interestProductId, Username disabledBy, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var interestProductAccountLinkEntities = await dbContext.InterestProductAccountLinks
            .AsTracking()
            .Where(x => x.IsActive == true && x.IsDeleted == false)
            .Where(x => x.InterestProductId == interestProductId)
            .ToListAsync(cancellationToken);

        foreach (var interestProductAccountLinkEntity in interestProductAccountLinkEntities)
        {
            interestProductAccountLinkEntity.IsActive = false;
            interestProductAccountLinkEntity.UpdatedBy = disabledBy;
            interestProductAccountLinkEntity.UpdatedAt = utcDateTime;
        }
    }
}