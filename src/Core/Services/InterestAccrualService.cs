using Core.Configs;
using Core.Constants;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Microsoft.Extensions.Options;

namespace Core.Services;

public class InterestAccrualService(
    IInterestAccrualRepository interestAccrualRepository,
    IInterestProductAccountLinkService interestProductAccountLinkService,
    ITransactionService transactionService,
    ICurrencyService currencyService,
    IBalanceService balanceService,
    IUnitOfWork unitOfWork,
    IOptions<AppConfig> appConfig,
    TimeProvider timeProvider) : IInterestAccrualService
{
    private readonly InterestAccrualJob _interestAccrualJob = appConfig.Value.InterestAccrualJob;

    public async Task AccrueMissingDaysAsync(CancellationToken cancellationToken)
    {
        var utcToday = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var lastAccruedAt = await interestAccrualRepository.GetLastAccrualDateAsync(cancellationToken);
        var lastAccruedDate = lastAccruedAt ?? utcToday.AddDays(-1);

        for (var date = lastAccruedDate.AddDays(1); date <= utcToday; date = date.AddDays(1))
        {
            await AccrueForDateAsync(date, cancellationToken);
        }
    }

    private async Task AccrueForDateAsync(DateOnly date, CancellationToken cancellationToken)
    {
        if (await interestAccrualRepository.ExistsByDateAsync(date, cancellationToken))
        {
            return;
        }

        var utcDateTime = timeProvider.GetUtcNow();
        var payoutCandidates = new Dictionary<Account, List<InterestAccrual>>();

        var interestProductAccountLinks = await interestProductAccountLinkService.GetActiveAsync(cancellationToken);
        foreach (var interestProductAccountLink in interestProductAccountLinks)
        {
            var interestProduct = interestProductAccountLink.InterestProduct;
            var account = interestProductAccountLink.Account;

            var balanceRequest = new BalanceRequest
            {
                AccountId = account.AccountId,
                DateTime = utcDateTime
            };

            var eligibleBalance = await balanceService.GetEligibleBalanceAsync(
                balanceRequest,
                _interestAccrualJob.InterestEligibilityLag,
                cancellationToken);

            var dailyInterestRate = interestProduct.AnnualInterestRate /
                                    interestProduct.AccrualBasis;

            var accruedAmount = currencyService.Round(
                account.CurrencyCode,
                eligibleBalance * dailyInterestRate);

            var interestAccrual = new InterestAccrual
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
                CreatedBy = new Username(SystemConstants.Username),
                UpdatedAt = new UpdatedAt(utcDateTime),
                UpdatedBy = new Username(SystemConstants.Username),
                IsDeleted = false,
                DeletedAt = null,
                DeletedBy = null
            };

            await interestAccrualRepository.CreateAsync(interestAccrual, cancellationToken);

            if (interestProduct.InterestPayoutFrequency == InterestPayoutFrequency.Daily)
            {
                payoutCandidates[account] = [interestAccrual];
            }
            else if (ShouldPayoutToday(interestProduct.InterestPayoutFrequency, DateOnly.FromDateTime(utcDateTime.UtcDateTime)))
            {
                var unpostedInterestAccruals = await interestAccrualRepository.GetUnpostedAsync(
                    account.AccountId,
                    interestProduct.InterestProductId,
                    cancellationToken);

                if (unpostedInterestAccruals.Count > 0)
                {
                    payoutCandidates[account] = unpostedInterestAccruals;
                }
            }
        }

        if (payoutCandidates.Count != 0)
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

        foreach (var interestAccruals in accruedInterest)
        {
            var createTransactionRequest = new CreateTransactionRequest
            {
                AccountId = interestAccruals.Key.AccountId,
                Amount = new TransactionAmount(interestAccruals.Value.Sum(y => y.AccruedAmount)),
                CurrencyCode = interestAccruals.Key.CurrencyCode,
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

            foreach (var interestAccrual in interestAccruals.Value)
            {
                await interestAccrualRepository.PostAsync(
                    interestAccrual.InterestAccrualId,
                    interestAccrual.CreatedBy,
                    cancellationToken);
            }
        }
    }
    
    private static bool ShouldPayoutToday(InterestPayoutFrequency interestPayoutFrequency, DateOnly date) => interestPayoutFrequency switch
    {
        InterestPayoutFrequency.Daily => true,
        InterestPayoutFrequency.Weekly => date.DayOfWeek == DayOfWeek.Sunday,
        InterestPayoutFrequency.Monthly => date.Day == 1,
        InterestPayoutFrequency.Quarterly => date is { Day: 1, Month: 1 or 4 or 7 or 10 },
        InterestPayoutFrequency.Yearly => date is { Day: 1, Month: 1 },
        _ => throw new ArgumentOutOfRangeException(nameof(interestPayoutFrequency), interestPayoutFrequency, null)
    };
}