using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;

namespace Infrastructure.Mappers;

public static class InterestAccrualMapper
{
    public static InterestAccrualEntity ToEntity(this InterestAccrual interestAccrual)
    {
        return new InterestAccrualEntity
        {
            InterestAccrualId = interestAccrual.InterestAccrualId,
            AccountId = interestAccrual.AccountId,
            InterestProductId = interestAccrual.InterestProductId,
            DailyInterestRate = interestAccrual.DailyInterestRate,
            AccruedAt = interestAccrual.AccruedAt,
            AccruedAmount = interestAccrual.AccruedAmount,
            IsPosted = interestAccrual.IsPosted,
            PostedAt = interestAccrual.PostedAt,
            CreatedAt = interestAccrual.CreatedAt,
            CreatedBy = interestAccrual.CreatedBy,
            UpdatedAt = interestAccrual.UpdatedAt,
            UpdatedBy = interestAccrual.UpdatedBy,
            IsDeleted = interestAccrual.IsDeleted,
            DeletedAt = interestAccrual.DeletedAt,
            DeletedBy = interestAccrual.DeletedBy,
        };
    }
    
    public static InterestAccrual ToModel(this InterestAccrualEntity interestAccrualEntity)
    {
        return new InterestAccrual
        {
            InterestAccrualId = new InterestAccrualId(interestAccrualEntity.InterestAccrualId),
            AccountId = new AccountId(interestAccrualEntity.AccountId),
            InterestProductId = new InterestProductId(interestAccrualEntity.InterestProductId),
            DailyInterestRate = new InterestRate(interestAccrualEntity.DailyInterestRate),
            AccruedAt = new AccruedAt(interestAccrualEntity.AccruedAt),
            AccruedAmount = new AccruedAmount(interestAccrualEntity.AccruedAmount),
            IsPosted = interestAccrualEntity.IsPosted,
            PostedAt = interestAccrualEntity.PostedAt.HasValue
                ? new PostedAt(interestAccrualEntity.PostedAt.Value)
                : null,
            CreatedAt = new CreatedAt(interestAccrualEntity.CreatedAt),
            CreatedBy = new Username(interestAccrualEntity.CreatedBy),
            UpdatedAt = new UpdatedAt(interestAccrualEntity.UpdatedAt),
            UpdatedBy = new Username(interestAccrualEntity.UpdatedBy),
            IsDeleted = interestAccrualEntity.IsDeleted,
            DeletedAt = interestAccrualEntity.DeletedAt.HasValue
                ? new DeletedAt(interestAccrualEntity.DeletedAt.Value)
                : null,
            DeletedBy = !string.IsNullOrWhiteSpace(interestAccrualEntity.DeletedBy)
                ? new Username(interestAccrualEntity.DeletedBy)
                : null
        };
    }
}