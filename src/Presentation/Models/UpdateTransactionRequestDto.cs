using Core.Enums;

namespace Presentation.Models;

public record UpdateTransactionRequestDto
{
    public required TransactionType Type { get; init; }
    public required TransactionSource Source { get; init; }
    public string? Description { get; init; }
    public string? Reference { get; init; }
}
