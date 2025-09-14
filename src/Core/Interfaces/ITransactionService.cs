using Core.Models;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface ITransactionService
{
    Task<Transaction> CreateAsync(CreateTransactionRequest createTransactionRequest, CancellationToken cancellationToken);
    Task<List<Transaction>> CreateManyAsync(List<CreateTransactionRequest> createTransactionRequests, CancellationToken cancellationToken);
    Task<Transaction> GetByIdAsync(TransactionId transactionId, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(TransactionId transactionId, CancellationToken cancellationToken);
    Task PostAsync(TransactionId transaction, Username postedBy, CancellationToken cancellationToken);
    Task DeleteAsync(TransactionId transactionId, Username deletedBy, CancellationToken cancellationToken);
    Task<Transaction> UpdateAsync(TransactionId transactionId, UpdateTransactionRequest updateTransactionRequest, CancellationToken cancellationToken);
    Task<Transaction> ReverseAsync(TransactionId transactionId, Username reversedBy, CancellationToken cancellationToken);
    Task<PagedResults<Transaction>> QueryAsync(QueryTransactionsRequest queryTransactionsRequest, CancellationToken cancellationToken);
}