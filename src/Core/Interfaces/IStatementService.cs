using Core.Models;

namespace Core.Interfaces;

public interface IStatementService
{
    Task<Statement> GetAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken);
    Task<byte[]> GeneratePdfAsync(GenerateStatementRequest generateStatementRequest, CancellationToken cancellationToken);
    Task<byte[]> GenerateCsvAsync(GenerateStatementRequest generatedStatementRequest, CancellationToken cancellationToken);
}