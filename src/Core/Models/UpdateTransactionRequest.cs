using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record UpdateTransactionRequest
{
    public required TransactionType Type { get; init; }
    public required TransactionSource Source { get; init; }
    public required TransactionDescription? Description { get; init; }
    public required TransactionReference? Reference { get; init; }
    public required Username UpdatedBy { get; init; }
}
