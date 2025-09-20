using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Extensions;
using Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TransactionRepository(
    ApplicationDbContext dbContext,
    TimeProvider timeProvider) : ITransactionRepository
{
    public async Task CreateAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        var transactionEntity = transaction.ToEntity();
        await dbContext.Transactions.AddAsync(transactionEntity, cancellationToken);
    }

    public async Task<Transaction?> GetByIdAsync(TransactionId transactionId, CancellationToken cancellationToken)
    {
        var transactionEntity = await dbContext.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TransactionId == transactionId && x.IsDeleted == false, cancellationToken);

        return transactionEntity?.ToModel();
    }

    public Task<bool> ExistsAsync(TransactionId transactionId, CancellationToken cancellationToken)
    {
        return dbContext.Transactions
            .AsNoTracking()
            .AnyAsync(x => x.TransactionId == transactionId && x.IsDeleted == false, cancellationToken);
    }

    public async Task PostAsync(TransactionId transactionId, Username postedBy, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var transactionEntity = await dbContext.Transactions
            .AsTracking()
            .FirstAsync(x => x.TransactionId == transactionId, cancellationToken);

        transactionEntity.TransactionStatusId = (int)TransactionStatus.Posted;
        transactionEntity.PostedAt = utcDateTime.UtcDateTime;
        transactionEntity.UpdatedBy = postedBy;
        transactionEntity.UpdatedAt = utcDateTime;
    }

    public async Task DeleteAsync(TransactionId transactionId, Username deletedBy,CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var transactionEntity = await dbContext.Transactions
            .AsTracking()
            .FirstAsync(x => x.TransactionId == transactionId, cancellationToken);

        transactionEntity.IsDeleted = true;
        transactionEntity.DeletedBy = deletedBy;
        transactionEntity.DeletedAt = utcDateTime;
    }

    public async Task<Transaction> UpdateAsync(UpdateTransactionRequest updateTransactionRequest, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var transactionEntity = await dbContext.Transactions
            .AsTracking()
            .FirstAsync(x => x.TransactionId == updateTransactionRequest.TransactionId, cancellationToken);

        transactionEntity.TransactionTypeId = (int)updateTransactionRequest.Type;
        transactionEntity.TransactionSourceId = (int)updateTransactionRequest.Source;
        transactionEntity.Description = updateTransactionRequest.Description;
        transactionEntity.Reference = updateTransactionRequest.Reference;
        transactionEntity.UpdatedBy = updateTransactionRequest.UpdatedBy;
        transactionEntity.UpdatedAt = utcDateTime;

        return transactionEntity.ToModel();
    }

    public async Task ReverseAsync(TransactionId transactionId, Username reversedBy, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var transactionEntity = await dbContext.Transactions
            .AsTracking()
            .FirstAsync(x => x.TransactionId == transactionId, cancellationToken);

        transactionEntity.TransactionStatusId = (int)TransactionStatus.Reversed;
        transactionEntity.UpdatedBy = reversedBy;
        transactionEntity.UpdatedAt = utcDateTime;
    }

    public Task<int> CountAsync(QueryTransactionsRequest queryTransactionsRequest, CancellationToken cancellationToken)
    {
        var query = dbContext.Transactions.BuildSearchQuery(queryTransactionsRequest);

        return query.CountAsync(cancellationToken);
    }

    public async Task<List<Transaction>> QueryAsync(QueryTransactionsRequest queryTransactionsRequest, CancellationToken cancellationToken)
    {
        var query = dbContext.Transactions.BuildSearchQuery(queryTransactionsRequest);

        var entities = await query
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.TransactionId)
            .Skip((queryTransactionsRequest.PageNumber - 1) * queryTransactionsRequest.PageSize)
            .Take(queryTransactionsRequest.PageSize)
            .ToListAsync(cancellationToken);

        return entities.Select(x => x.ToModel()).ToList();
    }
}