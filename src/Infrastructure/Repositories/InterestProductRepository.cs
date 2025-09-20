using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Extensions;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class InterestProductRepository(
    ApplicationDbContext dbContext,
    TimeProvider timeProvider) : IInterestProductRepository
{
    public async Task CreateAsync(InterestProduct interestProduct, CancellationToken cancellationToken)
    {
        var interestProductEntity = interestProduct.ToEntity();
        await dbContext.InterestProducts.AddAsync(interestProductEntity, cancellationToken);
    }

    public async Task<InterestProduct?> GetByIdAsync(InterestProductId interestProductId, CancellationToken cancellationToken)
    {
        var interestProductEntity = await dbContext.InterestProducts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.InterestProductId == interestProductId && x.IsDeleted == false, cancellationToken);

        return interestProductEntity?.ToModel();
    }

    public async Task DeleteAsync(InterestProductId interestProductId, Username deletedBy, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var interestProductEntity = await dbContext.InterestProducts
            .AsTracking()
            .FirstAsync(x => x.InterestProductId == interestProductId, cancellationToken);

        interestProductEntity.IsDeleted = true;
        interestProductEntity.DeletedBy = deletedBy;
        interestProductEntity.DeletedAt = utcDateTime;
    }

    public Task<bool> ExistsAsync(InterestProductId interestProductId, CancellationToken cancellationToken)
    {
        return dbContext.InterestProducts
            .AsNoTracking()
            .AnyAsync(x => x.InterestProductId ==  interestProductId && x.IsDeleted == false, cancellationToken);

    }

    public Task<int> CountAsync(QueryInterestProductsRequest queryInterestProductsRequest, CancellationToken cancellationToken)
    {
        var query = dbContext.InterestProducts.BuildSearchQuery(queryInterestProductsRequest);

        return query.CountAsync(cancellationToken);
    }

    public async Task<List<InterestProduct>> QueryAsync(QueryInterestProductsRequest queryInterestProductsRequest, CancellationToken cancellationToken)
    {
        var query = dbContext.InterestProducts.BuildSearchQuery(queryInterestProductsRequest);

        var entities = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.InterestProductId)
            .Skip((queryInterestProductsRequest.PageNumber - 1) * queryInterestProductsRequest.PageSize)
            .Take(queryInterestProductsRequest.PageSize)
            .ToListAsync(cancellationToken);

        return entities.Select(x => x.ToModel()).ToList();
    }

    public async Task<InterestProduct> UpdateAsync(
        UpdateInterestProductRequest updateInterestProductRequest,
        CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var interestProductEntity = await dbContext.InterestProducts
            .AsTracking()
            .FirstAsync(x => x.InterestProductId == updateInterestProductRequest.InterestProductId, cancellationToken);

        interestProductEntity.Name = updateInterestProductRequest.Name;
        interestProductEntity.AnnualInterestRate = updateInterestProductRequest.AnnualInterestRate;
        interestProductEntity.InterestPayoutFrequencyId = (int)updateInterestProductRequest.InterestPayoutFrequency;
        interestProductEntity.AccrualBasis = updateInterestProductRequest.AccrualBasis;
        interestProductEntity.UpdatedBy = updateInterestProductRequest.UpdatedBy;
        interestProductEntity.UpdatedAt = utcDateTime;

        return interestProductEntity.ToModel();
    }
}