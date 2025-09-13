using Core.Models;

namespace Core.Interfaces;

public interface IStatementService
{
    Task<Statement> GetAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken);
}