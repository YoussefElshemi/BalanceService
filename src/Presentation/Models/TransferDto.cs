namespace Presentation.Models;

public record TransferDto
{
    public required TransactionDto DebitTransaction { get; init; }
    public required TransactionDto CreditTransaction { get; init; }
}