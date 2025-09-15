using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record GenerateStatementRequest
{
    public required AccountId AccountId { get; init; }
    public required Range<DateOnly> DateRange { get; init; }
    public required StatementDirection? Direction { get; init; }
}