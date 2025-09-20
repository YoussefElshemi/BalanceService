using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IInterestProductRepository
{
    Task CreateAsync(InterestProduct interestProduct, CancellationToken cancellationToken);
    Task<InterestProduct?> GetByIdAsync(InterestProductId interestProductId, CancellationToken cancellationToken);
    Task DeleteAsync(InterestProductId interestProductId, Username deletedBy, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(InterestProductId interestProductId, CancellationToken cancellationToken);
    Task<int> CountAsync(QueryInterestProductsRequest queryInterestProductsRequest, CancellationToken cancellationToken);
    Task<List<InterestProduct>> QueryAsync(QueryInterestProductsRequest queryInterestProductsRequest, CancellationToken cancellationToken);
    Task<InterestProduct> UpdateAsync(UpdateInterestProductRequest updateInterestProductRequest, CancellationToken cancellationToken);
}