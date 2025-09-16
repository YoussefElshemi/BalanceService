using Core.Models;

namespace Core.Interfaces;

public interface IStatementRepository
{
    Task<int> CountAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken);
    Task<List<StatementEntry>> QueryAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken);
    Task<List<StatementEntry>> QueryAllAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken);
}