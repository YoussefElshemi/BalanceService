using Core.Models;
using Presentation.Models;

namespace Presentation.Mappers;

public static class InterestProductMapper
{
    public static InterestProductDto ToDto(this InterestProduct interestProduct)
    {
        return new InterestProductDto
        {
            InterestProductId = interestProduct.InterestProductId,
            Name = interestProduct.Name,
            AnnualInterestRate = interestProduct.AnnualInterestRate,
            InterestPayoutFrequency = interestProduct.InterestPayoutFrequency,
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
}