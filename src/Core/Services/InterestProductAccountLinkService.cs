using Core.Interfaces;
using Core.Models;

namespace Core.Services;

public class InterestProductAccountLinkService(
    IInterestProductAccountLinkRepository interestProductAccountLinkRepository) : IInterestProductAccountLinkService
{
    public Task<List<InterestProductAccountLink>> GetActiveAsync(CancellationToken cancellationToken)
    {
        return interestProductAccountLinkRepository.GetActiveAsync(cancellationToken);
    }
}