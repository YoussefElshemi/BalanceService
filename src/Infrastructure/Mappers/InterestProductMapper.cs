using Core.Enums;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;

namespace Infrastructure.Mappers;

public static class InterestProductMapper
{
    public static InterestProductEntity ToEntity(this InterestProduct interestProduct)
    {
        return new InterestProductEntity
        {
            InterestProductId = interestProduct.InterestProductId,
            Name = interestProduct.Name,
            AnnualInterestRate = interestProduct.AnnualInterestRate,
            InterestPayoutFrequencyId = (int)interestProduct.InterestPayoutFrequency,
            AccrualBasis = interestProduct.AccrualBasis,
            CreatedAt = interestProduct.CreatedAt,
            CreatedBy = interestProduct.CreatedBy,
            UpdatedAt = interestProduct.UpdatedAt,
            UpdatedBy = interestProduct.UpdatedBy,
            IsDeleted = interestProduct.IsDeleted,
            DeletedAt = interestProduct.DeletedAt,
            DeletedBy = interestProduct.DeletedBy
        };
    }
    
    public static InterestProduct ToModel(this InterestProductEntity interestProductEntity)
    {
        return new InterestProduct
        {
            InterestProductId = new InterestProductId(interestProductEntity.InterestProductId),
            Name = new InterestProductName(interestProductEntity.Name),
            AnnualInterestRate = new InterestRate(interestProductEntity.AnnualInterestRate),
            InterestPayoutFrequency = (InterestPayoutFrequency)interestProductEntity.InterestPayoutFrequencyId,
            AccrualBasis = new AccrualBasis(interestProductEntity.AccrualBasis),
            CreatedAt = new CreatedAt(interestProductEntity.CreatedAt),
            CreatedBy = new Username(interestProductEntity.CreatedBy),
            UpdatedAt = new UpdatedAt(interestProductEntity.UpdatedAt),
            UpdatedBy = new Username(interestProductEntity.UpdatedBy),
            IsDeleted = interestProductEntity.IsDeleted,
            DeletedAt = interestProductEntity.DeletedAt.HasValue
                ? new DeletedAt(interestProductEntity.DeletedAt.Value)
                : null,
            DeletedBy = !string.IsNullOrWhiteSpace(interestProductEntity.DeletedBy)
                ? new Username(interestProductEntity.DeletedBy)
                : null
        };
    }
}