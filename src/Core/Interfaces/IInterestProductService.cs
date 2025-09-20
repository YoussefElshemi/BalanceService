using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IInterestProductService
{
    Task<InterestProduct> CreateAsync(CreateInterestProductRequest createInterestProductRequest, CancellationToken cancellationToken);
    Task<InterestProduct> GetByIdAsync(InterestProductId interestProductId, CancellationToken cancellationToken);
    Task DeleteAsync(InterestProductId interestProductId, Username deletedBy, CancellationToken cancellationToken);
    Task<PagedResults<InterestProduct>> QueryAsync(QueryInterestProductsRequest queryInterestProductsRequest, CancellationToken cancellationToken);
    Task<InterestProduct> UpdateAsync(UpdateInterestProductRequest updateInterestProductRequest, CancellationToken cancellationToken);
}