using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.Services;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InterestProductAccountLinkRepository(ApplicationDbContext dbContext) : IInterestProductAccountLinkRepository
{
    public async Task<List<InterestProductAccountLink>> GetActiveAsync(CancellationToken cancellationToken)
    {
        var entities = await dbContext.InterestProductAccountLinks
            .AsNoTracking()
            .Where(x => x.IsActive == true && x.IsDeleted == false)
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
}