using System.ComponentModel;
using Core.Enums;

namespace Presentation.Models;

public record GetStatementRequestDto
{
    [DefaultValue(20)]
    public required int PageSize { get; init; } = 20;
    [DefaultValue(1)]
    public required int PageNumber { get; init; } = 1;
    public required Guid AccountId { get; init; }
    public required DateOnly FromDate  { get; init; }
    public required DateOnly ToDate { get; init; }
    public StatementDirection? Direction { get; init; }
}
