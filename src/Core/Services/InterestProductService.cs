using System.Diagnostics;
using Core.Constants;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;

namespace Core.Services;

public class InterestProductService(
    IInterestProductRepository interestProductRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IInterestProductService
{
    public async Task<InterestProduct> CreateAsync(
        CreateInterestProductRequest createInterestProductRequest,
        CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var interestProduct = new InterestProduct
        {
            InterestProductId = new InterestProductId(Guid.NewGuid()),
            Name = createInterestProductRequest.Name,
            AnnualInterestRate = createInterestProductRequest.AnnualInterestRate,
            InterestPayoutFrequency = createInterestProductRequest.InterestPayoutFrequency,
            AccrualBasis = createInterestProductRequest.AccrualBasis,
            CreatedAt = new CreatedAt(utcDateTime),
            CreatedBy = createInterestProductRequest.CreatedBy,
            UpdatedAt = new UpdatedAt(utcDateTime),
            UpdatedBy = createInterestProductRequest.CreatedBy,
            IsDeleted = false,
            DeletedAt = null,
            DeletedBy = null
        };
        
        Activity.Current?.AddTag(OpenTelemetryTags.Service.InterestProductId, interestProduct.InterestProductId.ToString());

        await interestProductRepository.CreateAsync(interestProduct, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return interestProduct;
    }

    public async Task<InterestProduct> GetByIdAsync(InterestProductId interestProductId, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.InterestProductId, interestProductId.ToString());

        var interestProduct = await interestProductRepository.GetByIdAsync(interestProductId, cancellationToken)
                              ?? throw new NotFoundException();

        return interestProduct;
    }

    public async Task DeleteAsync(InterestProductId interestProductId, Username deletedBy, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.InterestProductId, interestProductId.ToString());

        if (!await interestProductRepository.ExistsAsync(interestProductId, cancellationToken))
        {
            throw new NotFoundException();
        }

        await interestProductRepository.DeleteAsync(interestProductId, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResults<InterestProduct>> QueryAsync(QueryInterestProductsRequest queryInterestProductsRequest, CancellationToken cancellationToken)
    {
        var count = await interestProductRepository.CountAsync(queryInterestProductsRequest, cancellationToken);
        var interestProducts = await interestProductRepository.QueryAsync(queryInterestProductsRequest, cancellationToken);

        return new PagedResults<InterestProduct>
        {
            Data = interestProducts,
            MetaData = new PagedMetadata
            {
                TotalRecords = count,
                TotalPages = (count + queryInterestProductsRequest.PageSize - 1) / queryInterestProductsRequest.PageSize,
                PageSize = queryInterestProductsRequest.PageSize,
                PageNumber = queryInterestProductsRequest.PageNumber
            }
        };
    }

    public async Task<InterestProduct> UpdateAsync(
        InterestProductId interestProductId,
        UpdateInterestProductRequest updateInterestProductRequest,
        CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.InterestProductId, interestProductId.ToString());

        if (!await interestProductRepository.ExistsAsync(interestProductId, cancellationToken))
        {
            throw new NotFoundException();
        }

        var interestProduct = await interestProductRepository.UpdateAsync(interestProductId, updateInterestProductRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return interestProduct;
    }
}