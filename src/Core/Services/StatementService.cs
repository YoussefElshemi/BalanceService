using Core.Interfaces;
using Core.Models;

namespace Core.Services;

public class StatementService(
    IStatementRepository statementRepository) : IStatementService
{
    public async Task<Statement> GetAsync(GetStatementRequest getStatementRequest, CancellationToken cancellationToken)
    {
        var openingBalanceRequest = new BalanceRequest
        {
            AccountId = getStatementRequest.AccountId,
            Date = getStatementRequest.DateRange.From
        };

        var closingBalanceRequest = new BalanceRequest
        {
            AccountId = getStatementRequest.AccountId,
            Date = getStatementRequest.DateRange.To
        };

        var openingBalance = await statementRepository.GetAvailableBalanceAtAsync(openingBalanceRequest, cancellationToken);
        var closingBalance = await statementRepository.GetAvailableBalanceAtAsync(closingBalanceRequest, cancellationToken);

        var count = await statementRepository.CountAsync(getStatementRequest, cancellationToken);
        var statementEntries = await statementRepository.QueryAsync(getStatementRequest, cancellationToken);

        return new Statement
        {
            AccountId = getStatementRequest.AccountId,
            DateRange = getStatementRequest.DateRange,
            OpeningBalance = openingBalance,
            ClosingBalance = closingBalance,
            StatementEntries = new PagedResults<StatementEntry>
            {
                Data = statementEntries,
                MetaData = new PagedMetadata
                {
                    TotalRecords = count,
                    TotalPages = (count + getStatementRequest.PageSize - 1) / getStatementRequest.PageSize,
                    PageSize = getStatementRequest.PageSize,
                    PageNumber = getStatementRequest.PageNumber
                }
            }
        };
    }
}