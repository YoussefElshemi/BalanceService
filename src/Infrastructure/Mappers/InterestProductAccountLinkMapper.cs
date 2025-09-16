using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;

namespace Infrastructure.Mappers;

public static class InterestProductAccountLinkMapper
{
    public static InterestProductAccountLinkEntity ToEntity(this InterestProductAccountLink interestProductAccountLink)
    {
        return new InterestProductAccountLinkEntity
        {
            InterestProductId = interestProductAccountLink.InterestProduct.InterestProductId,
            AccountId = interestProductAccountLink.Account.AccountId,
            IsActive = interestProductAccountLink.IsActive,
            CreatedAt = interestProductAccountLink.CreatedAt,
            CreatedBy = interestProductAccountLink.CreatedBy,
            UpdatedAt = interestProductAccountLink.UpdatedAt,
            UpdatedBy = interestProductAccountLink.UpdatedBy,
            IsDeleted = interestProductAccountLink.IsDeleted,
            DeletedAt = interestProductAccountLink.DeletedAt,
            DeletedBy = interestProductAccountLink.DeletedBy
        };
    }
    
    public static InterestProductAccountLink ToModel(this InterestProductAccountLinkEntity interestProductAccountLinkEntity)
    {
        return new InterestProductAccountLink
        {
            InterestProduct = interestProductAccountLinkEntity.InterestProductEntity.ToModel(),
            Account = interestProductAccountLinkEntity.AccountEntity.ToModel(),
            IsActive = interestProductAccountLinkEntity.IsActive,
            CreatedAt = new CreatedAt(interestProductAccountLinkEntity.CreatedAt),
            CreatedBy = new Username(interestProductAccountLinkEntity.CreatedBy),
            UpdatedAt = new UpdatedAt(interestProductAccountLinkEntity.UpdatedAt),
            UpdatedBy = new Username(interestProductAccountLinkEntity.UpdatedBy),
            IsDeleted = interestProductAccountLinkEntity.IsDeleted,
            DeletedAt = interestProductAccountLinkEntity.DeletedAt.HasValue
                ? new DeletedAt(interestProductAccountLinkEntity.DeletedAt.Value)
                : null,
            DeletedBy = !string.IsNullOrWhiteSpace(interestProductAccountLinkEntity.DeletedBy)
                ? new Username(interestProductAccountLinkEntity.DeletedBy)
                : null
        };
    }
}