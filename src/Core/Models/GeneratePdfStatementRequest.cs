using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record GeneratePdfStatementRequest
{
    public required AccountId AccountId { get; init; }
    public required Range<DateOnly> DateRange { get; init; }
    public required StatementDirection? Direction { get; init; }
}