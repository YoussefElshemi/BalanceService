using Core.Models;
using Presentation.Mappers.InterestProducts;
using Presentation.Models.InterestProductAccountLinks;

namespace Presentation.Mappers.InterestProductAccountLinks;

public static class InterestProductAccountLinkMapper
{
    public static InterestProductAccountLinkDto ToDto(this InterestProductAccountLink interestProductAccountLink)
    {
        return new InterestProductAccountLinkDto
        {
            AccountId = interestProductAccountLink.Account.AccountId,
            InterestProduct = interestProductAccountLink.InterestProduct.ToDto(),
            IsActive = interestProductAccountLink.IsActive,
            ExpiresAt = interestProductAccountLink.ExpiresAt,
            CreatedAt = interestProductAccountLink.CreatedAt,
            CreatedBy = interestProductAccountLink.CreatedBy,
            UpdatedAt = interestProductAccountLink.UpdatedAt,
            UpdatedBy = interestProductAccountLink.UpdatedBy,
            IsDeleted = interestProductAccountLink.IsDeleted,
            DeletedAt = interestProductAccountLink.DeletedAt,
            DeletedBy = interestProductAccountLink.DeletedBy
        };
    }
}