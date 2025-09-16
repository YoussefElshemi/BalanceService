using Core.Models;

namespace Core.Interfaces;

public interface IInterestProductAccountLinkService
{
   Task<List<InterestProductAccountLink>> GetActiveAsync(CancellationToken cancellationToken);
}