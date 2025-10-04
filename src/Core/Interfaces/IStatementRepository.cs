using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IStatementRepository
{
    Task<int> CountAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken);

    Task<List<StatementEntry>> QueryAsync(
        GetStatementRequest getStatementRequest,
        AvailableBalance openingBalance,
        CancellationToken cancellationToken);

    Task<List<StatementEntry>> QueryAllAsync(
        GetStatementRequest getStatementRequest,
        AvailableBalance openingBalance,
        CancellationToken cancellationToken);
}