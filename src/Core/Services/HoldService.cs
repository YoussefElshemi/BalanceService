using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;

namespace Core.Services;

public class HoldService(
    IHoldRepository holdRepository,
    ITransactionRepository transactionRepository,
    IAccountRulesService accountRulesService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IHoldService
{
    public async Task<Hold> CreateAsync(CreateHoldRequest createHoldRequest,
        CancellationToken cancellationToken)
    {
        await accountRulesService.ThrowIfNotAllowedAsync(createHoldRequest.AccountId, AccountOperationType.CreateHold, cancellationToken);

        var utcDateTime = timeProvider.GetUtcNow();

        var hold = new Hold
        {
            HoldId = new HoldId(Guid.NewGuid()),
            AccountId = createHoldRequest.AccountId,
            Amount = createHoldRequest.Amount,
            CurrencyCode = createHoldRequest.CurrencyCode,
            IdempotencyKey = createHoldRequest.IdempotencyKey,
            Type = createHoldRequest.Type,
            Status = createHoldRequest.Status,
            Source = createHoldRequest.Source,
            ExpiresAt = createHoldRequest.ExpiresAt,
            SettledTransactionId = null,
            Description = createHoldRequest.Description,
            Reference = createHoldRequest.Reference,
            CreatedAt = new CreatedAt(utcDateTime),
            CreatedBy = createHoldRequest.CreatedBy,
            UpdatedAt = new UpdatedAt(utcDateTime),
            UpdatedBy = createHoldRequest.CreatedBy,
            IsDeleted = false,
            DeletedAt = null,
            DeletedBy = null
        };

        await holdRepository.CreateAsync(hold, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return hold;
    }

    public async Task<Hold> GetByIdAsync(HoldId holdId, CancellationToken cancellationToken)
    {
        var hold =  await holdRepository.GetByIdAsync(holdId, cancellationToken) ;

        if (hold is null)
        {
            throw new NotFoundException();
        }

        return hold;
    }

    public async Task ReleaseAsync(HoldId holdId, Username releasedBy, CancellationToken cancellationToken)
    {
        var hold = await GetByIdAsync(holdId, cancellationToken);

        if (hold.Status != HoldStatus.Active)
        {
            throw new UnprocessableRequestException($"{nameof(Hold)} must be in a {nameof(HoldStatus.Active)} status to be released");
        }

        await accountRulesService.ThrowIfNotAllowedAsync(hold.AccountId, AccountOperationType.ReleaseHold, cancellationToken);

        await holdRepository.ReleaseAsync(hold.HoldId, releasedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(HoldId holdId, Username deletedBy, CancellationToken cancellationToken)
    {
        var hold = await GetByIdAsync(holdId, cancellationToken);

        if (hold.Status != HoldStatus.Active)
        {
            throw new UnprocessableRequestException($"{nameof(Hold)} must be in a {nameof(HoldStatus.Active)} status to be deleted");
        }

        await holdRepository.DeleteAsync(holdId, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<Transaction> SettleAsync(HoldId holdId, Username settledBy, CancellationToken cancellationToken)
    {
        var hold = await GetByIdAsync(holdId, cancellationToken);

        if (hold.Status != HoldStatus.Active)
        {
            throw new UnprocessableRequestException($"{nameof(Hold)} must be in a {nameof(HoldStatus.Active)} status to be settled");
        }

        await accountRulesService.ThrowIfNotAllowedAsync(hold.AccountId, AccountOperationType.SettleHold, cancellationToken);

        var utcDateTime = timeProvider.GetUtcNow();

        var transaction = new Transaction
        {
            TransactionId = new TransactionId(Guid.NewGuid()),
            AccountId = hold.AccountId,
            Amount = new TransactionAmount(hold.Amount),
            CurrencyCode = hold.CurrencyCode,
            Direction = TransactionDirection.Debit,
            PostedAt = new PostedAt(utcDateTime),
            IdempotencyKey = hold.IdempotencyKey,
            Type = TransactionType.SettledHold,
            Status = TransactionStatus.Posted,
            Source = MapSourceFromHoldToTransaction(hold.Source),
            Description = hold.Description.HasValue 
                ? new TransactionDescription(hold.Description)
                : null,
            Reference = hold.Reference.HasValue
                ? new TransactionReference(hold.Reference)
                : null,
            CreatedAt = new CreatedAt(utcDateTime),
            CreatedBy = settledBy,
            UpdatedAt = new UpdatedAt(utcDateTime),
            UpdatedBy = settledBy,
            IsDeleted = false,
            DeletedAt = null,
            DeletedBy = null
        };

        await transactionRepository.CreateAsync(transaction, cancellationToken);
        await holdRepository.SettleAsync(hold.HoldId, transaction.TransactionId, settledBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return transaction;
    }

    public async Task<Hold> UpdateAsync(HoldId holdId, UpdateHoldRequest updateHoldRequest, CancellationToken cancellationToken)
    {
        var hold = await GetByIdAsync(holdId, cancellationToken);

        if (hold.Status != HoldStatus.Active)
        {
            throw new UnprocessableRequestException($"{nameof(Hold)} must be in a {nameof(HoldStatus.Active)} status to be updated");
        }

        var updatedHold = await holdRepository.UpdateAsync(holdId, updateHoldRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        return updatedHold;
    }

    public async Task<PagedResults<Hold>> QueryAsync(QueryHoldsRequest queryHoldsRequest, CancellationToken cancellationToken)
    {
        var count = await holdRepository.CountAsync(queryHoldsRequest, cancellationToken);
        var holds = await holdRepository.QueryAsync(queryHoldsRequest, cancellationToken);

        return new PagedResults<Hold>
        {
            Data = holds,
            MetaData = new PagedMetadata
            {
                TotalRecords = count,
                TotalPages = (count + queryHoldsRequest.PageSize - 1) / queryHoldsRequest.PageSize,
                PageSize = queryHoldsRequest.PageSize,
                PageNumber = queryHoldsRequest.PageNumber
            }
        };
    }

    public async Task ExpireHoldsAsync(CancellationToken cancellationToken)
    {
        await holdRepository.ExpireHoldsAsync(cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static TransactionSource MapSourceFromHoldToTransaction(HoldSource holdSource) => holdSource switch
    {
        HoldSource.Api => TransactionSource.Api,
        HoldSource.Import => TransactionSource.Import,
        HoldSource.Manual => TransactionSource.Manual,
        _ => throw new ArgumentOutOfRangeException(nameof(holdSource), holdSource, null)
    };
}