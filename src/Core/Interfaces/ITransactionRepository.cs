using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface ITransactionRepository
{
    Task CreateAsync(Transaction transaction, CancellationToken cancellationToken);
    Task<Transaction?> GetByIdAsync(TransactionId transactionId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(TransactionId transactionId, CancellationToken cancellationToken);
    Task PostAsync(TransactionId transactionId, Username postedBy, CancellationToken cancellationToken);
    Task DeleteAsync(TransactionId transactionId, Username deletedBy, CancellationToken cancellationToken);
    Task<Transaction> UpdateAsync(TransactionId transactionId, UpdateTransactionRequest updateTransactionRequest, CancellationToken cancellationToken);
    Task ReverseAsync(TransactionId transactionId, Username reversedBy, CancellationToken cancellationToken);
    Task<int> CountAsync(QueryTransactionsRequest queryTransactionsRequest, CancellationToken cancellationToken);
    Task<List<Transaction>> QueryAsync(QueryTransactionsRequest queryTransactionsRequest, CancellationToken cancellationToken);
}