using Core.Dtos;
using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record AccountHistory : Account, IHistory<Guid, HistoryDto<AccountHistoryDto>>
{
    public Guid GetPrimaryKey() => AccountHistoryId;
    public required AccountHistoryId AccountHistoryId { get; init; }
    public required HistoryType HistoryType { get; init; }
    public required Timestamp Timestamp { get; init; }
    public required bool IsProcessed { get; init; }
    public required ProcessedAt? ProcessedAt { get; init; }

    public HistoryDto<AccountHistoryDto> ToDto()
    {
        return new HistoryDto<AccountHistoryDto>
        {
            HistoryId = AccountHistoryId,
            Type = HistoryType,
            Timestamp = Timestamp,
            Data = new AccountHistoryDto
            {
                AccountId = AccountId,
                AccountName = AccountName,
                CurrencyCode = CurrencyCode,
                AvailableBalance = AvailableBalance,
                LedgerBalance = LedgerBalance,
                PendingBalance = PendingBalance,
                HoldBalance = HoldBalance,
                MinimumRequiredBalance = MinimumRequiredBalance,
                AccountType = AccountType,
                AccountStatus = AccountStatus,
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