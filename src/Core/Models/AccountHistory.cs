using Core.Dtos;
using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record AccountHistory : Account, IHistory<HistoryDto<AccountHistoryDto>>
{
    public Guid GetPrimaryKey() => AccountHistoryId;
    public Guid GetKey() => AccountId;
    public required AccountHistoryId AccountHistoryId { get; init; }
    public required HistoryType HistoryType { get; init; }
    public required Timestamp Timestamp { get; init; }

    public HistoryDto<AccountHistoryDto> ToDto()
    {
        return new HistoryDto<AccountHistoryDto>
        {
            IdempotencyKey = AccountHistoryId,
            Type = HistoryType,
            Timestamp = Timestamp,
            Data = new AccountHistoryDto
            {
                AccountId = AccountId,
                AccountName = AccountName,
                CurrencyCode = CurrencyCode,
                AvailableBalance = AvailableBalance,
                LedgerBalance = LedgerBalance,
                PendingDebitBalance = PendingDebitBalance,
                PendingCreditBalance = PendingCreditBalance,
                HoldBalance = HoldBalance,
                MinimumRequiredBalance = MinimumRequiredBalance,
                AccountType = Type,
                AccountStatus = Status,
                Metadata = Metadata,
                ParentAccountId = ParentAccountId,
                CreatedAt = CreatedAt,
                CreatedBy = CreatedBy,
                UpdatedAt = UpdatedAt,
                UpdatedBy = UpdatedBy,
                IsDeleted = IsDeleted,
                DeletedAt = DeletedAt,
                DeletedBy = DeletedBy
            }
        };
    }
}