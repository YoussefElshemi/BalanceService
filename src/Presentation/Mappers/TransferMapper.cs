using Core.Models;
using Presentation.Models;

namespace Presentation.Mappers;

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