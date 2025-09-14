using System.Text.Json;
using Core.Enums;
using Core.Models;
using Core.ValueObjects;

namespace Infrastructure.Entities.History;

public record AccountHistoryEntity : IHistoryEntity<AccountHistory>
{
    public static string GetIdColumn() => nameof(AccountId);
    public required Guid AccountHistoryId { get; init; }
    public required int HistoryTypeId { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required Guid AccountId { get; init; }
    public required string AccountName { get; init; }
    public required string CurrencyCode { get; init; }
    public required decimal LedgerBalance { get; init; }
    public required decimal AvailableBalance { get; init; }
    public required decimal PendingBalance { get; init; }
    public required decimal HoldBalance { get; init; }
    public required decimal MinimumRequiredBalance { get; init; }
    public required int AccountTypeId { get; init; }
    public required int AccountStatusId { get; init; }
    public required string? Metadata { get; init; }
    public required Guid? ParentAccountId { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTimeOffset? DeletedAt { get; init; }
    public required string? DeletedBy { get; init; }

    public required int ProcessingStatusId { get; set; }
    public required DateTimeOffset? ProcessedAt { get; set; }

    public uint RowVersion { get; init; }

    public AccountHistory ToModel()
    {
        return new AccountHistory
        {
            AccountHistoryId = new AccountHistoryId(AccountHistoryId),
            HistoryType = (HistoryType)HistoryTypeId,
            Timestamp = new Timestamp(Timestamp),
            AccountId = new AccountId(AccountId),
            AccountName = new AccountName(AccountName),
            CurrencyCode = Enum.Parse<CurrencyCode>(CurrencyCode),
            AvailableBalance = new AvailableBalance(AvailableBalance),
            LedgerBalance = new LedgerBalance(LedgerBalance),
            PendingBalance = new PendingBalance(PendingBalance),
            HoldBalance = new HoldBalance(HoldBalance),
            MinimumRequiredBalance = new MinimumRequiredBalance(MinimumRequiredBalance),
            AccountType = (AccountType)AccountTypeId,
            AccountStatus = (AccountStatus)AccountStatusId,
            Metadata = !string.IsNullOrWhiteSpace(Metadata)
                ? JsonDocument.Parse(Metadata)
                : null,
            ParentAccountId = ParentAccountId.HasValue
                ? new AccountId(ParentAccountId.Value)
                : null,
            CreatedAt = new CreatedAt(CreatedAt),
            CreatedBy = new Username(CreatedBy),
            UpdatedAt = new UpdatedAt(UpdatedAt),
            UpdatedBy = new  Username(UpdatedBy),
            IsDeleted = IsDeleted,
            DeletedAt = DeletedAt.HasValue
                ? new DeletedAt(DeletedAt.Value)
                : null,
            DeletedBy = !string.IsNullOrWhiteSpace(DeletedBy)
                ? new Username(DeletedBy)
                : null
        };
    }

    public static string[] GetColumns() => [
        nameof(AccountId),
        nameof(AccountName),
        nameof(CurrencyCode),
        nameof(LedgerBalance),
        nameof(AvailableBalance),
        nameof(PendingBalance),
        nameof(HoldBalance),
        nameof(MinimumRequiredBalance),
        nameof(AccountTypeId),
        nameof(AccountStatusId),
        nameof(Metadata),
        nameof(ParentAccountId),
        nameof(IsDeleted),
        nameof(DeletedAt),
        nameof(DeletedBy),
        nameof(CreatedAt),
        nameof(CreatedBy),
        nameof(UpdatedAt),
        nameof(UpdatedBy)
    ];

    public AccountEntity? ParentAccountEntity { get; init; }
    public AccountTypeEntity AccountTypeEntity { get; init; } = null!;
    public AccountStatusEntity AccountStatusEntity { get; init; } = null!;
    public HistoryTypeEntity HistoryTypeEntity { get; init; } = null!;
    public ProcessingStatusEntity ProcessingStatusEntity { get; init; } = null!;
}