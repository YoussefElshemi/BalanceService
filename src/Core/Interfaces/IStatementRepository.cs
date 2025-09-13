using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IStatementRepository
{
    Task<AvailableBalance> GetAvailableBalanceAtAsync(BalanceRequest balanceRequest, CancellationToken cancellationToken);
    Task<int> CountAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken);
    Task<List<StatementEntry>> QueryAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken);
}