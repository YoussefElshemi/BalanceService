using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IBalanceRepository
{
    Task<AvailableBalance> GetAvailableBalanceAsync(BalanceRequest balanceRequest, CancellationToken cancellationToken);
    Task<AvailableBalance> GetEligibleBalanceAsync(BalanceRequest balanceRequest, TimeSpan lag, CancellationToken cancellationToken);
}