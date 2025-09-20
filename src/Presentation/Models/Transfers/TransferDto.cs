using Presentation.Models.Transactions;

namespace Presentation.Models.Transfers;

public record TransferDto
{
    public required TransactionDto DebitTransaction { get; init; }
    public required TransactionDto CreditTransaction { get; init; }
}