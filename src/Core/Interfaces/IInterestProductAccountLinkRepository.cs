using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IInterestProductAccountLinkRepository
{
   Task CreateAsync(InterestProductAccountLink interestProductAccountLink, CancellationToken cancellationToken);
   Task<InterestProductAccountLink?> GetByIdAsync(AccountId accountId, InterestProductId interestProductId, CancellationToken cancellationToken);
   Task UpdateActiveAsync(UpdateInterestProductAccountLinkActiveRequest updateInterestProductAccountLinkActiveRequest, CancellationToken cancellationToken);
   Task<List<InterestProductAccountLink>> GetActiveAsync(CancellationToken cancellationToken);
   Task<bool> ExistsAsync(AccountId accountId, InterestProductId interestProductId, CancellationToken cancellationToken);
   Task<InterestProductAccountLink> UpdateAsync(UpdateInterestProductAccountLinkRequest updateInterestProductAccountLinkRequest, CancellationToken cancellationToken);
   Task DeleteAsync(AccountId accountId, InterestProductId interestProductId, Username deletedBy, CancellationToken cancellationToken);
   Task<int> CountAsync(QueryInterestProductAccountLinksRequest queryInterestProductAccountLinksRequest, CancellationToken cancellationToken);
   Task<List<InterestProductAccountLink>> QueryAsync(QueryInterestProductAccountLinksRequest queryInterestProductAccountLinksRequest, CancellationToken cancellationToken);
   Task MarkAsInactiveAsync(InterestProductId interestProductId, Username disabledBy, CancellationToken cancellationToken);
}