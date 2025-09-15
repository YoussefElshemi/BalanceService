using Core.Enums;

namespace Presentation.Models;

public record GenerateStatementRequestDto
{
    public required Guid AccountId { get; init; }
    public required DateOnly FromDate  { get; init; }
    public required DateOnly ToDate { get; init; }
    public StatementDirection? Direction { get; init; }
}
