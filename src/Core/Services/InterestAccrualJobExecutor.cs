using Core.Configs;
using Core.Constants;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Microsoft.Extensions.Options;

namespace Core.Services;

public class InterestAccrualJobExecutor(
    IInterestAccrualRepository interestAccrualRepository,
    IInterestProductAccountLinkService interestProductAccountLinkService,
    ITransactionService transactionService,
    ICurrencyService currencyService,
    IBalanceService balanceService,
    IUnitOfWork unitOfWork,
    IOptions<AppConfig> appConfig,
    TimeProvider timeProvider) : IJobExecutor
{
    private readonly InterestAccrualJobConfig _interestAccrualJobConfig = appConfig.Value.InterestAccrualJobConfig;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();
        var payoutCandidates = new Dictionary<Account, List<InterestAccrual>>();

        var links = await interestProductAccountLinkService.GetActiveAsync(cancellationToken);
        foreach (var link in links)
        {
            var interestProduct = link.InterestProduct;
            var account = link.Account;

            var balanceRequest = new BalanceRequest
            {
                AccountId = account.AccountId,
                DateTime = utcDateTime
            };

            var eligibleBalance = await balanceService.GetEligibleBalanceAsync(
                balanceRequest,
                _interestAccrualJobConfig.InterestEligibilityLag,
                cancellationToken);

            var dailyInterestRate = interestProduct.AnnualInterestRate / interestProduct.AccrualBasis;
            var accruedAmount = currencyService.Round(account.CurrencyCode, eligibleBalance * dailyInterestRate);

            var accrual = new InterestAccrual
            {
                InterestAccrualId = new InterestAccrualId(Guid.NewGuid()),
                AccountId = account.AccountId,
                InterestProductId = interestProduct.InterestProductId,
                AccruedAt = new AccruedAt(utcDateTime),
                DailyInterestRate = new InterestRate(dailyInterestRate),
                AccruedAmount = new AccruedAmount(accruedAmount),
                IsPosted = false,
                PostedAt = null,
                CreatedAt = new CreatedAt(utcDateTime),
                CreatedBy = SystemConstants.Username,
                UpdatedAt = new UpdatedAt(utcDateTime),
                UpdatedBy = SystemConstants.Username,
                IsDeleted = false,
                DeletedAt = null,
                DeletedBy = null
            };

            await interestAccrualRepository.CreateAsync(accrual, cancellationToken);

            if (interestProduct.InterestPayoutFrequency == InterestPayoutFrequency.Daily ||
                ShouldPayoutToday(
                    interestProduct.InterestPayoutFrequency,
                    DateOnly.FromDateTime(utcDateTime.UtcDateTime)))
            {
                var unposted = interestProduct.InterestPayoutFrequency == InterestPayoutFrequency.Daily
                    ? [accrual]
                    : await interestAccrualRepository.GetUnpostedAsync(
                        account.AccountId,
                        interestProduct.InterestProductId,
                        cancellationToken);

                if (unposted.Count > 0)
                {
                    payoutCandidates[account] = unposted;
                }
            }
        }

        if (payoutCandidates.Count > 0)
        {
            await PayoutAccruedInterestAsync(payoutCandidates, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task PayoutAccruedInterestAsync(
        Dictionary<Account, List<InterestAccrual>> accruedInterest,
        CancellationToken cancellationToken)
    {
        var utcDateTime = timeProvider.GetUtcNow();

        foreach (var (account, interestAccruals) in accruedInterest)
        {
            var createTransactionRequest = new CreateTransactionRequest
            {
                AccountId = account.AccountId,
                Amount = new TransactionAmount(interestAccruals.Sum(x => x.AccruedAmount)),
                CurrencyCode = account.CurrencyCode,
                Direction = TransactionDirection.Credit,
                IdempotencyKey = new IdempotencyKey(Guid.NewGuid()),
                Type = TransactionType.AccruedInterest,
                Source = TransactionSource.Internal,
                Status = TransactionStatus.Posted,
                PostedAt = new PostedAt(utcDateTime),
                Description = new TransactionDescription($"Interest {utcDateTime.ToString(DateTimeConstants.DateFormat)}"),
                Reference = new TransactionReference($"Interest {utcDateTime.ToString(DateTimeConstants.DateFormat)}"),
                CreatedBy = new Username(SystemConstants.Username)
            };

            await transactionService.CreateAsync(createTransactionRequest, cancellationToken);

            foreach (var interestAccrual in interestAccruals)
            {
                await interestAccrualRepository.PostAsync(
                    interestAccrual.InterestAccrualId,
                    interestAccrual.CreatedBy,
                    cancellationToken);
            }
        }
    }

    private static bool ShouldPayoutToday(InterestPayoutFrequency frequency, DateOnly date) => frequency switch
    {
        InterestPayoutFrequency.Weekly => date.DayOfWeek == DayOfWeek.Sunday,
        InterestPayoutFrequency.Monthly => date.Day == 1,
        InterestPayoutFrequency.Quarterly => date is { Day: 1, Month: 1 or 4 or 7 or 10 },
        InterestPayoutFrequency.Yearly => date is { Day: 1, Month: 1 },
        _ => false
    };
}
