using System.ComponentModel;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Statements;

public record GetStatementRequestDto : BaseReadRequestDto
{
    [DefaultValue(20)]
    [FromQuery]
    public required int PageSize { get; init; } = 20;
    [DefaultValue(1)]
    [FromQuery]
    public required int PageNumber { get; init; } = 1;
    
    [FromQuery]
    public required Guid AccountId { get; init; }
    
    [FromQuery]
    public required DateOnly FromDate  { get; init; }
    
    [FromQuery]
    public required DateOnly ToDate { get; init; }
    
    [FromQuery]
    public StatementDirection? Direction { get; init; }
}
