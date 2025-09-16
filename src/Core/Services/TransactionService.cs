using System.Diagnostics;
using Core.Constants;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;

namespace Core.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    IHistoryService<TransactionHistory> transactionHistoryService,
    IAccountRulesService accountRulesService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ITransactionService
{
    public async Task<Transaction> CreateAsync(CreateTransactionRequest createTransactionRequest, CancellationToken cancellationToken)
    {
        var transactions = await CreateManyAsync([createTransactionRequest], cancellationToken);

        return transactions.Single();
    }

    public async Task<List<Transaction>> CreateManyAsync(List<CreateTransactionRequest> createTransactionRequests, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var transactions = new List<Transaction>();

        Activity.Current?.AddTag(OpenTelemetryTags.Service.AccountId, string.Join(", ", createTransactionRequests.Select(x => x.AccountId.ToString())));

        // TODO: this will select account from db for each one, optimise

        foreach (var createTransactionRequest in createTransactionRequests)
        {
            var operationType = createTransactionRequest.Direction == TransactionDirection.Credit
                ? AccountOperationType.CreditTransaction
                : AccountOperationType.DebitTransaction;

            await accountRulesService.ThrowIfNotAllowedAsync(createTransactionRequest.AccountId, operationType, cancellationToken);

            var transaction = new Transaction
            {
                TransactionId = new TransactionId(Guid.NewGuid()),
                AccountId = createTransactionRequest.AccountId,
                Amount = createTransactionRequest.Amount,
                CurrencyCode = createTransactionRequest.CurrencyCode,
                Direction = createTransactionRequest.Direction,
                PostedAt = createTransactionRequest.PostedAt,
                IdempotencyKey = createTransactionRequest.IdempotencyKey,
                Type = createTransactionRequest.Type,
                Status = createTransactionRequest.Status,
                Source = createTransactionRequest.Source,
                Description = createTransactionRequest.Description,
                Reference = createTransactionRequest.Reference,
                CreatedAt = new CreatedAt(utcDateTime),
                CreatedBy = createTransactionRequest.CreatedBy,
                UpdatedAt = new UpdatedAt(utcDateTime),
                UpdatedBy = createTransactionRequest.CreatedBy,
                IsDeleted = false,
                DeletedAt = null,
                DeletedBy = null
            };

            await transactionRepository.CreateAsync(transaction, cancellationToken);
            transactions.Add(transaction);
        }

        Activity.Current?.AddTag(OpenTelemetryTags.Service.TransactionId, string.Join(", " , transactions.Select(x => x.TransactionId.ToString())));

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return transactions;
    }

    public async Task<Transaction> GetByIdAsync(TransactionId transactionId, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.TransactionId, transactionId.ToString());

        var transaction = await transactionRepository.GetByIdAsync(transactionId, cancellationToken)
                          ?? throw new NotFoundException();

        return transaction;
    }

    public Task<bool> ExistsAsync(TransactionId transactionId, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.TransactionId, transactionId.ToString());
        return transactionRepository.ExistsAsync(transactionId, cancellationToken);
    }

    public async Task<Transaction> UpdateAsync(TransactionId transactionId, UpdateTransactionRequest updateTransactionRequest, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.TransactionId, transactionId.ToString());

        var transaction = await GetByIdAsync(transactionId, cancellationToken);
        
        if (transaction.Status != TransactionStatus.Draft)
        {
            throw new UnprocessableRequestException($"{nameof(Transaction)} must be in a {nameof(TransactionStatus.Draft)} status to be updated");
        }

        var updatedTransaction = await transactionRepository.UpdateAsync(transactionId, updateTransactionRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return updatedTransaction;
    }

    public async Task<Transaction> ReverseAsync(TransactionId transactionId, Username reversedBy, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.TransactionId, transactionId.ToString());

        var transaction = await GetByIdAsync(transactionId, cancellationToken);

        if (transaction.Status != TransactionStatus.Posted)
        {
            throw new UnprocessableRequestException($"{nameof(Transaction)} must be in a {nameof(TransactionStatus.Posted)} status to be reversed");
        }

        var reversedDirection = transaction.Direction == TransactionDirection.Credit
            ? TransactionDirection.Debit
            : TransactionDirection.Credit;

        var operationType = reversedDirection == TransactionDirection.Credit
            ? AccountOperationType.CreditTransaction
            : AccountOperationType.DebitTransaction;

        await accountRulesService.ThrowIfNotAllowedAsync(transaction.AccountId, operationType, cancellationToken);

        var utcDateTime = timeProvider.GetUtcNow();

        var reversedTransaction = transaction with
        {
            TransactionId = new TransactionId(Guid.NewGuid()),
            Direction = reversedDirection,
            IdempotencyKey = new IdempotencyKey(Guid.NewGuid()),
            PostedAt = new PostedAt(utcDateTime),
            Status = TransactionStatus.Posted,
            CreatedAt = new CreatedAt(utcDateTime),
            CreatedBy = reversedBy,
            UpdatedAt = new UpdatedAt(utcDateTime),
            UpdatedBy = reversedBy,
            IsDeleted = false,
            DeletedAt = null,
            DeletedBy = null
        };

        Activity.Current?.AddTag(OpenTelemetryTags.Service.ReversedTransactionId, reversedTransaction.TransactionId.ToString());

        await transactionRepository.CreateAsync(reversedTransaction, cancellationToken);
        await transactionRepository.ReverseAsync(transaction.TransactionId, reversedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return reversedTransaction;
    }

    public async Task<PagedResults<Transaction>> QueryAsync(QueryTransactionsRequest queryTransactionsRequest, CancellationToken cancellationToken)
    {
        var count = await transactionRepository.CountAsync(queryTransactionsRequest, cancellationToken);
        var transactions = await transactionRepository.QueryAsync(queryTransactionsRequest, cancellationToken);

        return new PagedResults<Transaction>
        {
            Data = transactions,
            MetaData = new PagedMetadata
            {
                TotalRecords = count,
                TotalPages = (count + queryTransactionsRequest.PageSize - 1) / queryTransactionsRequest.PageSize,
                PageSize = queryTransactionsRequest.PageSize,
                PageNumber = queryTransactionsRequest.PageNumber
            }
        };
    }

    public async Task<PagedResults<ChangeEvent>> GetHistoryAsync(GetChangesRequest getChangesRequest, CancellationToken cancellationToken)
    {
        if (!await transactionRepository.ExistsAsync(new TransactionId(getChangesRequest.EntityId), cancellationToken))
        {
            throw new NotFoundException();
        }
        
        var count = await transactionHistoryService.CountChangesAsync(getChangesRequest, cancellationToken);
        var changeEvents = await transactionHistoryService.GetChangesAsync(getChangesRequest, cancellationToken);

        return new PagedResults<ChangeEvent>
        {
            Data = changeEvents,
            MetaData = new PagedMetadata
            {
                TotalRecords = count,
                TotalPages = (count + getChangesRequest.PageSize - 1) / getChangesRequest.PageSize,
                PageSize = getChangesRequest.PageSize,
                PageNumber = getChangesRequest.PageNumber
            }
        };
    }

    public async Task PostAsync(TransactionId transactionId, Username postedBy, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.TransactionId, transactionId.ToString());

        var transaction = await GetByIdAsync(transactionId, cancellationToken);

        if (transaction.Status != TransactionStatus.Draft)
        {
            throw new UnprocessableRequestException($"{nameof(Transaction)} must be in a {nameof(TransactionStatus.Draft)} status to be posted");
        }

        var operationType = transaction.Direction == TransactionDirection.Credit
            ? AccountOperationType.CreditTransaction
            : AccountOperationType.DebitTransaction;

        await accountRulesService.ThrowIfNotAllowedAsync(transaction.AccountId, operationType, cancellationToken);

        await transactionRepository.PostAsync(transaction.TransactionId, postedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TransactionId transactionId, Username deletedBy, CancellationToken cancellationToken)
    {
        Activity.Current?.AddTag(OpenTelemetryTags.Service.TransactionId, transactionId.ToString());

        var transaction = await GetByIdAsync(new TransactionId(transactionId), cancellationToken);

        if (transaction.Status != TransactionStatus.Draft)
        {
            throw new UnprocessableRequestException($"{nameof(Transaction)} must be in a {nameof(TransactionStatus.Draft)} status to be deleted");
        }
        
        await transactionRepository.DeleteAsync(transactionId, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}