using Core.Enums;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;

namespace Infrastructure.Mappers;

public static class HoldMapper
{
    public static HoldEntity ToEntity(this Hold hold)
    {
        return new HoldEntity
        {
            HoldId = hold.HoldId,
            AccountId = hold.AccountId,
            Amount = hold.Amount,
            CurrencyCode = hold.CurrencyCode.ToString(),
            IdempotencyKey = hold.IdempotencyKey,
            HoldTypeId = (int)hold.Type,
            HoldStatusId = (int)hold.Status,
            HoldSourceId = (int)hold.Source,
            SettledTransactionId = hold.SettledTransactionId,
            ExpiresAt = hold.ExpiresAt,
            Description = hold.Description,
            Reference = hold.Reference,
            CreatedAt = hold.CreatedAt,
            CreatedBy = hold.CreatedBy,
            UpdatedAt = hold.UpdatedAt,
            UpdatedBy = hold.UpdatedBy,
            IsDeleted = hold.IsDeleted,
            DeletedAt = hold.DeletedAt,
            DeletedBy = hold.DeletedBy
        };
    }
    
    public static Hold ToModel(this HoldEntity holdEntity)
    {
        return new Hold
        {
            HoldId = new HoldId(holdEntity.HoldId),
            AccountId = new AccountId(holdEntity.AccountId),
            Amount = new HoldAmount(holdEntity.Amount),
            CurrencyCode = Enum.Parse<CurrencyCode>(holdEntity.CurrencyCode),
            IdempotencyKey = new IdempotencyKey(Guid.NewGuid()),
            Type = (HoldType)holdEntity.HoldTypeId,
            Status = (HoldStatus)holdEntity.HoldStatusId,
            Source = (HoldSource)holdEntity.HoldSourceId,
            SettledTransactionId = holdEntity.SettledTransactionId.HasValue
                ? new TransactionId(holdEntity.SettledTransactionId.Value)
                : null,
            ExpiresAt = holdEntity.ExpiresAt.HasValue
                ? new ExpiresAt(holdEntity.ExpiresAt.Value)
                : null,
            Description = !string.IsNullOrWhiteSpace(holdEntity.Description)
                ? new HoldDescription(holdEntity.Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(holdEntity.Reference)
                ? new HoldReference(holdEntity.Reference)
                : null,
            CreatedAt = new CreatedAt(holdEntity.CreatedAt),
            CreatedBy = new Username(holdEntity.CreatedBy),
            UpdatedAt = new UpdatedAt(holdEntity.UpdatedAt),
            UpdatedBy = new  Username(holdEntity.UpdatedBy),
            IsDeleted = holdEntity.IsDeleted,
            DeletedAt = holdEntity.DeletedAt.HasValue
                ? new DeletedAt(holdEntity.DeletedAt.Value)
                : null,
            DeletedBy = !string.IsNullOrWhiteSpace(holdEntity.DeletedBy)
                ? new Username(holdEntity.DeletedBy)
                : null
        };
    }
}