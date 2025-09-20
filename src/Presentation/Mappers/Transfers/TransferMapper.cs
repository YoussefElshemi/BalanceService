using Core.Models;
using Presentation.Mappers.Transactions;
using Presentation.Models.Transfers;

namespace Presentation.Mappers.Transfers;

public static class TransferMapper
{
    public static TransferDto ToDto(this Transfer transfer)
    {
        return new TransferDto
        {
            CreditTransaction = transfer.CreditTransaction.ToDto(),
            DebitTransaction = transfer.DebitTransaction.ToDto()
        };
    }
}