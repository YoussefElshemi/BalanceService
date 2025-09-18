using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IInterestProductAccountLinkService
{
   Task<InterestProductAccountLink> CreateAsync(CreateInterestProductAccountLinkRequest createInterestProductAccountLinkRequest, CancellationToken cancellationToken);
   Task<InterestProductAccountLink> GetByIdAsync(AccountId accountId, InterestProductId interestProductId, CancellationToken cancellationToken);
   Task ActivateAsync(AccountId accountId, InterestProductId interestProductId, Username activatedBy, CancellationToken cancellationToken);
   Task DeactivateAsync(AccountId accountId, InterestProductId interestProductId, Username deactivatedBy, CancellationToken cancellationToken);
   Task<List<InterestProductAccountLink>> GetActiveAsync(CancellationToken cancellationToken);
   Task<InterestProductAccountLink> UpdateAsync(UpdateInterestProductAccountLinkRequest updateInterestProductAccountLinkRequest, CancellationToken cancellationToken);
   Task DeleteAsync(AccountId accountId, InterestProductId interestProductId, Username deletedBy, CancellationToken cancellationToken);
   Task<PagedResults<InterestProductAccountLink>> QueryAsync(QueryInterestProductAccountLinksRequest queryInterestProductAccountLinksRequest, CancellationToken cancellationToken);
}