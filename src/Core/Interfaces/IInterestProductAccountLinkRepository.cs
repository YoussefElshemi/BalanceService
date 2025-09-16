using Core.Models;

namespace Core.Interfaces;

public interface IInterestProductAccountLinkRepository
{
   Task<List<InterestProductAccountLink>> GetActiveAsync(CancellationToken cancellationToken);
}