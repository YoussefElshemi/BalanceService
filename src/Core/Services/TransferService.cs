using System.Diagnostics;
using Core.Constants;
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;

namespace Core.Services;

public class TransferService(
    ITransactionService transactionService,
    IAccountService accountService,
    IAccountRulesService accountRulesService,
    TimeProvider timeProvider) : ITransferService
{
    public async Task<Transfer> CreateAsync(CreateTransferRequest createTransferRequest, CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();

        Activity.Current?.AddTag(OpenTelemetryTags.Service.TransferCreditAccountId, createTransferRequest.CreditAccountId.ToString());
        Activity.Current?.AddTag(OpenTelemetryTags.Service.TransferDebitAccountId, createTransferRequest.DebitAccountId.ToString());

        var creditAccount = await accountService.GetByIdAsync(createTransferRequest.CreditAccountId, cancellationToken); 
        var debitAccount = await accountService.GetByIdAsync(createTransferRequest.DebitAccountId, cancellationToken);

        accountRulesService.ThrowIfNotAllowed(creditAccount.Status, AccountOperationType.CreditTransaction);
        accountRulesService.ThrowIfNotAllowed(debitAccount.Status, AccountOperationType.DebitTransaction);

        if (creditAccount.CurrencyCode != debitAccount.CurrencyCode)
        {
            throw new UnprocessableRequestException($"{nameof(Account)}s involved in a {nameof(Transfer)} must have the same {nameof(CurrencyCode)}");
        }

        var createDebitTransactionRequest = new CreateTransactionRequest
        {
            AccountId = createTransferRequest.DebitAccountId,
            Amount = new TransactionAmount(createTransferRequest.Amount),
            CurrencyCode = createTransferRequest.CurrencyCode,
            Direction = TransactionDirection.Debit,
            IdempotencyKey = createTransferRequest.DebitIdempotencyKey,
            Type = TransactionType.Transfer,
            Source = MapSourceFromTransferToTransaction(createTransferRequest.Source),
            Status = TransactionStatus.Posted,
            PostedAt = new PostedAt(utcDateTime),
            Description = !string.IsNullOrWhiteSpace(createTransferRequest.Description)
                ? new TransactionDescription(createTransferRequest.Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(createTransferRequest.Reference)
                ? new TransactionReference(createTransferRequest.Reference)
                : null,
            CreatedBy = createTransferRequest.CreatedBy
        };

        var createCreditTransactionRequest = createDebitTransactionRequest with
        {
            AccountId = createTransferRequest.CreditAccountId,
            Direction = TransactionDirection.Credit,
            IdempotencyKey = createTransferRequest.CreditIdempotencyKey
        };

        var transactions = await transactionService.CreateManyAsync(
            [
                createDebitTransactionRequest,
                createCreditTransactionRequest
            ],
            cancellationToken);

        return new Transfer
        {
            DebitTransaction = transactions.Single(x => x.Direction == TransactionDirection.Debit),
            CreditTransaction = transactions.Single(x => x.Direction == TransactionDirection.Credit)
        };
    }

    private static TransactionSource MapSourceFromTransferToTransaction(TransferSource transferSource) => transferSource switch
    {
        TransferSource.Api => TransactionSource.Api,
        TransferSource.Import => TransactionSource.Import,
        TransferSource.Manual => TransactionSource.Manual,
        _ => throw new ArgumentOutOfRangeException(nameof(transferSource), transferSource, null)
    };
}