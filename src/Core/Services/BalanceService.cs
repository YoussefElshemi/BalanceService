using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;

namespace Core.Services;

public class BalanceService(IBalanceRepository balanceRepository) : IBalanceService
{
    public Task<AvailableBalance> GetAvailableBalanceAsync(BalanceRequest balanceRequest, CancellationToken cancellationToken)
    {
        return balanceRepository.GetAvailableBalanceAsync(balanceRequest, cancellationToken);
    }

    public Task<AvailableBalance> GetEligibleBalanceAsync(BalanceRequest balanceRequest, TimeSpan lag, CancellationToken cancellationToken)
    {
        return balanceRepository.GetEligibleBalanceAsync(balanceRequest, lag, cancellationToken);
    }
}