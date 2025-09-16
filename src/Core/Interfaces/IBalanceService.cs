using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IBalanceService
{
    Task<AvailableBalance> GetAvailableBalanceAsync(BalanceRequest balanceRequest, CancellationToken cancellationToken);
    Task<AvailableBalance> GetEligibleBalanceAsync(BalanceRequest balanceRequest, TimeSpan lag, CancellationToken cancellationToken);
}