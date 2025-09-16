using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class BalanceRepository(ApplicationDbContext dbContext) : IBalanceRepository
{
    public async Task<AvailableBalance> GetAvailableBalanceAsync(BalanceRequest balanceRequest, CancellationToken cancellationToken)
    {
        var result = await dbContext.Transactions
            .Where(x => x.IsDeleted == false)
            .Where(x => x.AccountId == balanceRequest.AccountId)
            .Where(x =>x.PostedAt!.Value <= balanceRequest.DateTime)
            .Where(x => x.TransactionStatusId == (int)TransactionStatus.Posted ||
                        x.TransactionStatusId == (int)TransactionStatus.Reversed)
            .GroupBy(x => 1)
            .Select(x => new
            {
                LedgerBalance = x.Sum(y =>
                    y.TransactionDirectionId == (int)TransactionDirection.Credit ? y.Amount : -y.Amount),
                HoldBalance = dbContext.Holds
                    .Where(z => z.IsDeleted == false)
                    .Where(z => z.AccountId == balanceRequest.AccountId)
                    .Where(z =>z.CreatedAt <= balanceRequest.DateTime)
                    .Where(z => z.HoldStatusId == (int)HoldStatus.Active)
                    .Sum(z => z.Amount)
            })
            .SingleOrDefaultAsync(cancellationToken);

        return new AvailableBalance(result?.LedgerBalance - result?.HoldBalance ?? 0);
    }

    public async Task<AvailableBalance> GetEligibleBalanceAsync(
        BalanceRequest balanceRequest,
        TimeSpan lag,
        CancellationToken cancellationToken)
    {
        var result = await dbContext.Transactions
            .Where(x => x.IsDeleted == false)
            .Where(x => x.AccountId == balanceRequest.AccountId)
            .Where(x => x.TransactionStatusId == (int)TransactionStatus.Posted ||
                        x.TransactionStatusId == (int)TransactionStatus.Reversed)
            .GroupBy(x => 1)
            .Select(x => new
            {
                MaturedCredits = x
                    .Where(y => y.TransactionDirectionId == (int)TransactionDirection.Credit &&
                                y.PostedAt <= balanceRequest.DateTime - lag)
                    .Sum(y => y.Amount),

                Debits = x
                    .Where(y => y.TransactionDirectionId == (int)TransactionDirection.Debit &&
                                y.PostedAt <= balanceRequest.DateTime)
                    .Sum(y => y.Amount),

                HoldBalance = dbContext.Holds
                    .Where(z => z.IsDeleted == false)
                    .Where(z => z.AccountId == balanceRequest.AccountId)
                    .Where(z =>z.CreatedAt <= balanceRequest.DateTime)
                    .Where(z => z.HoldStatusId == (int)HoldStatus.Active)
                    .Sum(z => z.Amount)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var ledgerBalance = (result?.MaturedCredits ?? 0) - (result?.Debits ?? 0);
        var availableBalance = ledgerBalance - (result?.HoldBalance ?? 0);

        return new AvailableBalance(availableBalance);
    }
}