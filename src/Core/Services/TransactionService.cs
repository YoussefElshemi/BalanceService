using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;

namespace Core.Services;

public class TransactionService(
    ITransactionRepository transactionRepository,
    IAccountRulesService accountRulesService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ITransactionService
{
    public async Task<Transaction> CreateAsync(CreateTransactionRequest createTransactionRequest, CancellationToken cancellationToken)
    {
        var transactions = await CreateManyAsync([createTransactionRequest], cancellationToken);

        return transactions.Single();
    }

    public async Task<List<Transaction>> CreateManyAsync(IEnumerable<CreateTransactionRequest> createTransactionRequests, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var transactions = new List<Transaction>();

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
                PostedDate = createTransactionRequest.PostedDate,
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

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return transactions;
    }

    public async Task<Transaction> GetByIdAsync(TransactionId transactionId, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(transactionId, cancellationToken);

        if (transaction is null)
        {
            throw new NotFoundException();
        }

        return transaction;
    }

    public Task<bool> ExistsAsync(TransactionId transactionId, CancellationToken cancellationToken)
    {
        return transactionRepository.ExistsAsync(transactionId, cancellationToken);
    }

    public async Task<Transaction> UpdateAsync(TransactionId transactionId, UpdateTransactionRequest updateTransactionRequest, CancellationToken cancellationToken)
    {
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
            PostedDate = new PostedDate(DateOnly.FromDateTime(utcDateTime.UtcDateTime)),
            Status = TransactionStatus.Posted,
            CreatedAt = new CreatedAt(utcDateTime),
            CreatedBy = reversedBy,
            UpdatedAt = new UpdatedAt(utcDateTime),
            UpdatedBy = reversedBy,
            IsDeleted = false,
            DeletedAt = null,
            DeletedBy = null
        };

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

    public async Task PostAsync(TransactionId transactionId, Username postedBy, CancellationToken cancellationToken)
    {
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
        var transaction = await GetByIdAsync(new TransactionId(transactionId), cancellationToken);

        if (transaction.Status != TransactionStatus.Draft)
        {
            throw new UnprocessableRequestException($"{nameof(Transaction)} must be in a {nameof(TransactionStatus.Draft)} status to be deleted");
        }
        
        await transactionRepository.DeleteAsync(transactionId, deletedBy, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}