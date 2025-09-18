using System.Diagnostics;
using Core.Constants;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;

namespace Core.Services;

public class InterestProductAccountLinkService(
    IInterestProductAccountLinkRepository interestProductAccountLinkRepository,
    IAccountService accountService,
    IInterestProductService interestProductService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IInterestProductAccountLinkService
{
    public async Task<InterestProductAccountLink> CreateAsync(
        CreateInterestProductAccountLinkRequest createInterestProductAccountLinkRequest,
        CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        
        var account = await accountService.GetByIdAsync(createInterestProductAccountLinkRequest.AccountId, cancellationToken);
        var interestProduct = await interestProductService.GetByIdAsync(createInterestProductAccountLinkRequest.InterestProductId, cancellationToken);
        
        var interestProductAccountLink = new InterestProductAccountLink
        {
            InterestProduct = interestProduct,
            Account = account,
            IsActive = false,
            ExpiresAt = createInterestProductAccountLinkRequest.ExpiresAt,
            CreatedAt = new CreatedAt(utcDateTime),
            CreatedBy = createInterestProductAccountLinkRequest.CreatedBy,
            UpdatedAt = new UpdatedAt(utcDateTime),
            UpdatedBy = createInterestProductAccountLinkRequest.CreatedBy,
            IsDeleted = false,
            DeletedAt = null,
            DeletedBy = null
        };
        
        Activity.Current?.AddTag(OpenTelemetryTags.Service.InterestProductId, interestProductAccountLink.InterestProduct.InterestProductId.ToString());
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, interestProductAccountLink.Account.AccountId.ToString());

        await interestProductAccountLinkRepository.CreateAsync(interestProductAccountLink, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return interestProductAccountLink;
    }

    public async Task<InterestProductAccountLink> GetByIdAsync(AccountId accountId, InterestProductId interestProductId, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, accountId.ToString());
        Activity.Current?.AddTag(OpenTelemetryTags.Service.InterestProductId, interestProductId.ToString());

        var interestProductAccountLink = await interestProductAccountLinkRepository.GetByIdAsync(accountId, interestProductId, cancellationToken)
                                         ?? throw new NotFoundException();

        return interestProductAccountLink;
    }

    public async Task ActivateAsync(
        AccountId accountId,
        InterestProductId interestProductId,
        Username activatedBy,
        CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();

        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, accountId.ToString());
        Activity.Current?.AddTag(OpenTelemetryTags.Service.InterestProductId, interestProductId.ToString());

        var interestProductAccountLink = await GetByIdAsync(accountId, interestProductId, cancellationToken);
        if (interestProductAccountLink.IsActive)
        {
            throw new UnprocessableRequestException($"{nameof(InterestProductAccountLink)} must not be active");
        }

        if (interestProductAccountLink.InterestProduct.IsDeleted)
        {
            throw new UnprocessableRequestException($"{nameof(InterestProductAccountLink)} is deleted");
        }

        if (interestProductAccountLink.ExpiresAt >= utcDateTime)
        {
            throw new UnprocessableRequestException($"{nameof(InterestProductAccountLink)} is expired");
        }

        var updateInterestProductAccountLinkActiveRequest = new UpdateInterestProductAccountLinkActiveRequest
        {
            AccountId = accountId,
            InterestProductId = interestProductId,
            UpdatedBy = activatedBy,
            IsActive = true
        };

        await interestProductAccountLinkRepository.UpdateActiveAsync(updateInterestProductAccountLinkActiveRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateAsync(
        AccountId accountId,
        InterestProductId interestProductId,
        Username deactivatedBy,
        CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, accountId.ToString());
        Activity.Current?.AddTag(OpenTelemetryTags.Service.InterestProductId, interestProductId.ToString());

        var interestProductAccountLink = await GetByIdAsync(accountId, interestProductId, cancellationToken);
        if (!interestProductAccountLink.IsActive)
        {
            throw new UnprocessableRequestException($"{nameof(InterestProductAccountLink)} must be active");
        }

        var updateInterestProductAccountLinkActiveRequest = new UpdateInterestProductAccountLinkActiveRequest
        {
            AccountId = accountId,
            InterestProductId = interestProductId,
            UpdatedBy = deactivatedBy,
            IsActive = false
        };

        await interestProductAccountLinkRepository.UpdateActiveAsync(updateInterestProductAccountLinkActiveRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public Task<List<InterestProductAccountLink>> GetActiveAsync(CancellationToken cancellationToken)
    {
        return interestProductAccountLinkRepository.GetActiveAsync(cancellationToken);
    }

    public async Task<InterestProductAccountLink> UpdateAsync(
        UpdateInterestProductAccountLinkRequest updateInterestProductAccountLinkRequest,
        CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, updateInterestProductAccountLinkRequest.AccountId.ToString());
        Activity.Current?.AddTag(OpenTelemetryTags.Service.InterestProductId, updateInterestProductAccountLinkRequest.InterestProductId.ToString());

        if (!await interestProductAccountLinkRepository.ExistsAsync(
                updateInterestProductAccountLinkRequest.AccountId,
                updateInterestProductAccountLinkRequest.InterestProductId,
                cancellationToken))
        {
            throw new NotFoundException();
        }

        var interestProductAccountLink = await interestProductAccountLinkRepository.UpdateAsync(updateInterestProductAccountLinkRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return interestProductAccountLink;
    }

    public async Task DeleteAsync(
        AccountId accountId,
        InterestProductId interestProductId,
        Username deletedBy,
        CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, accountId.ToString());
        Activity.Current?.AddTag(OpenTelemetryTags.Service.InterestProductId, interestProductId.ToString());

        if (!await interestProductAccountLinkRepository.ExistsAsync(
                accountId,
                interestProductId,
                cancellationToken))
        {
            throw new NotFoundException();
        }

        await interestProductAccountLinkRepository.DeleteAsync(
            accountId,
            interestProductId,
            deletedBy,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResults<InterestProductAccountLink>> QueryAsync(
        QueryInterestProductAccountLinksRequest queryInterestProductAccountLinksRequest,
        CancellationToken cancellationToken)
    {
        var count = await interestProductAccountLinkRepository.CountAsync(queryInterestProductAccountLinksRequest, cancellationToken);
        var interestProductAccountLinks = await interestProductAccountLinkRepository.QueryAsync(queryInterestProductAccountLinksRequest, cancellationToken);

        return new PagedResults<InterestProductAccountLink>
        {
            Data = interestProductAccountLinks,
            MetaData = new PagedMetadata
            {
                TotalRecords = count,
                TotalPages = (count + queryInterestProductAccountLinksRequest.PageSize - 1) / queryInterestProductAccountLinksRequest.PageSize,
                PageSize = queryInterestProductAccountLinksRequest.PageSize,
                PageNumber = queryInterestProductAccountLinksRequest.PageNumber
            }
        };
    }
}