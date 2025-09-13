using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record GetStatementRequest
{
    public required PageSize PageSize { get; init; }
    public required PageNumber PageNumber { get; init; }
    public required AccountId AccountId { get; init; }
    public required Range<DateOnly> DateRange { get; init; }
    public required StatementDirection? Direction { get; init; }
}