using Core.Models;
using Presentation.Models.Transactions;

namespace Presentation.Mappers.Transactions;

public static class TransactionMapper
{
    public static TransactionDto ToDto(this Transaction transaction)
    {
        return new TransactionDto
        {
            TransactionId = transaction.TransactionId,
            AccountId = transaction.AccountId,
            Amount = transaction.Amount,
            CurrencyCode = transaction.CurrencyCode,
            Direction = transaction.Direction,
            PostedDate = transaction.PostedAt,
            IdempotencyKey = transaction.IdempotencyKey,
            Type = transaction.Type,
            Status = transaction.Status,
            Source = transaction.Source,
            Description = transaction.Description,
            Reference = transaction.Reference,
            CreatedAt = transaction.CreatedAt,
            CreatedBy = transaction.CreatedBy,
            UpdatedAt = transaction.UpdatedAt,
            UpdatedBy = transaction.UpdatedBy,
            IsDeleted = transaction.IsDeleted,
            DeletedAt = transaction.DeletedAt,
            DeletedBy = transaction.DeletedBy
        };
    }
}