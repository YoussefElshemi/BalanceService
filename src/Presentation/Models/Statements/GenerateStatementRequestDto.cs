using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Statements;

public record GenerateStatementRequestDto : BaseReadRequestDto
{
    [FromBody]
    public required Guid AccountId { get; init; }
    
    [FromBody]
    public required DateOnly FromDate  { get; init; }
    
    [FromBody]
    public required DateOnly ToDate { get; init; }
    
    [FromBody]
    public StatementDirection? Direction { get; init; }
}
