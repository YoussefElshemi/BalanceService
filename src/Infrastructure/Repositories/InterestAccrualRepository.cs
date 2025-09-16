using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InterestAccrualRepository(
    ApplicationDbContext dbContext,
    TimeProvider timeProvider) : IInterestAccrualRepository
{
    public async Task CreateAsync(InterestAccrual interestAccrual, CancellationToken cancellationToken)
    {
        var interestAccrualEntity = interestAccrual.ToEntity();
        await dbContext.InterestAccruals.AddAsync(interestAccrualEntity, cancellationToken);
    }

    public async Task<AccruedAt?> GetLastAccrualDateAsync(CancellationToken cancellationToken)
    {
        var lastAccrualDate = await dbContext.InterestAccruals
            .AsNoTracking()
            .MaxAsync(x => (DateTimeOffset?)x.AccruedAt, cancellationToken);

        return lastAccrualDate.HasValue
            ? new AccruedAt(lastAccrualDate.Value)
            : null;
    }

    public async Task<bool> ExistsByDateAsync(DateOnly date, CancellationToken cancellationToken)
    {
        return await dbContext.InterestAccruals
            .AsNoTracking()
            .AnyAsync(x => DateOnly.FromDateTime(x.AccruedAt.UtcDateTime) == date, cancellationToken);
    }

    public async Task PostAsync(InterestAccrualId interestAccrualId, Username postedBy, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var transactionEntity = await dbContext.InterestAccruals
            .AsTracking()
            .FirstAsync(x => x.InterestAccrualId == interestAccrualId, cancellationToken);

        transactionEntity.IsPosted = true;
        transactionEntity.PostedAt = utcDateTime.UtcDateTime;
        transactionEntity.UpdatedBy = postedBy;
        transactionEntity.UpdatedAt = utcDateTime;
    }

    public async Task<List<InterestAccrual>> GetUnpostedAsync(
        AccountId accountId,
        InterestProductId interestProductId,
        CancellationToken cancellationToken)
    {
        var entities = await dbContext.InterestAccruals
            .Where(x => x.IsDeleted == false)
            .Where(x => x.IsPosted == false)
            .Where(x => x.AccountId == accountId)
            .Where(x => x.InterestProductId == interestProductId)
            .ToListAsync(cancellationToken);
        
        return entities.Select(x => x.ToModel()).ToList();
    }
}