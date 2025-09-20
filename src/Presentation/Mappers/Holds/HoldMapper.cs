using Core.Models;
using Presentation.Models.Holds;

namespace Presentation.Mappers.Holds;

public static class HoldMapper
{
    public static HoldDto ToDto(this Hold hold)
    {
        return new HoldDto
        {
            HoldId = hold.HoldId,
            AccountId = hold.AccountId,
            Amount = hold.Amount,
            CurrencyCode = hold.CurrencyCode,
            IdempotencyKey = hold.IdempotencyKey,
            Type = hold.Type,
            Status = hold.Status,
            Source = hold.Source,
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
}