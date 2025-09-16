using Core.Enums;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;

namespace Infrastructure.Mappers;

public static class TransactionMapper
{
    public static TransactionEntity ToEntity(this Transaction transaction)
    {
        return new TransactionEntity
        {
            TransactionId = transaction.TransactionId,
            AccountId = transaction.AccountId,
            Amount = transaction.Amount,
            CurrencyCode = transaction.CurrencyCode.ToString(),
            TransactionDirectionId = (int)transaction.Direction,
            PostedAt = transaction.PostedAt,
            IdempotencyKey = transaction.IdempotencyKey,
            TransactionTypeId = (int)transaction.Type,
            TransactionStatusId = (int)transaction.Status,
            TransactionSourceId = (int)transaction.Source,
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
    
    public static Transaction ToModel(this TransactionEntity transactionEntity)
    {
        return new Transaction
        {
            TransactionId = new TransactionId(transactionEntity.TransactionId),
            AccountId = new AccountId(transactionEntity.AccountId),
            Amount = new TransactionAmount(transactionEntity.Amount),
            CurrencyCode = Enum.Parse<CurrencyCode>(transactionEntity.CurrencyCode),
            Direction = (TransactionDirection)transactionEntity.TransactionDirectionId,
            PostedAt = transactionEntity.PostedAt.HasValue
                ? new PostedAt(transactionEntity.PostedAt.Value)
                : null,
            IdempotencyKey = new IdempotencyKey(transactionEntity.IdempotencyKey),
            Type = (TransactionType)transactionEntity.TransactionTypeId,
            Status = (TransactionStatus)transactionEntity.TransactionStatusId,
            Source = (TransactionSource)transactionEntity.TransactionSourceId,
            Description = !string.IsNullOrWhiteSpace(transactionEntity.Description)
                ? new TransactionDescription(transactionEntity.Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(transactionEntity.Reference)
                ? new TransactionReference(transactionEntity.Reference)
                : null,
            CreatedAt = new CreatedAt(transactionEntity.CreatedAt),
            CreatedBy = new Username(transactionEntity.CreatedBy),
            UpdatedAt = new UpdatedAt(transactionEntity.UpdatedAt),
            UpdatedBy = new Username(transactionEntity.UpdatedBy),
            IsDeleted = transactionEntity.IsDeleted,
            DeletedAt = transactionEntity.DeletedAt.HasValue
                ? new DeletedAt(transactionEntity.DeletedAt.Value)
                : null,
            DeletedBy = !string.IsNullOrWhiteSpace(transactionEntity.DeletedBy)
                ? new Username(transactionEntity.DeletedBy)
                : null
        };
    }
}