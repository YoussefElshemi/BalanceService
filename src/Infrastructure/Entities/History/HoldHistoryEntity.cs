using Core.Enums;
using Core.Models;
using Core.ValueObjects;

namespace Infrastructure.Entities.History;

public record HoldHistoryEntity : IHistoryEntity<HoldHistory>
{
    public Guid GetPrimaryKey() => HoldHistoryId;
    public required Guid HoldHistoryId { get; init; }
    public required int HistoryTypeId { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required Guid HoldId { get; init; }
    public required Guid AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required Guid IdempotencyKey { get; init; }
    public required int HoldTypeId { get; init; }
    public required int HoldStatusId { get; init; }
    public required int HoldSourceId { get; init; }
    public required Guid? SettledTransactionId  { get; init; }
    public required DateTimeOffset? ExpiresAt { get; init; }
    public required string? Description { get; init; }
    public required string? Reference { get; init; }

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

    public HoldHistory ToModel()
    {
        return new HoldHistory
        {
            HoldHistoryId = new HoldHistoryId(HoldHistoryId),
            HistoryType = (HistoryType)HistoryTypeId,
            Timestamp = new Timestamp(Timestamp),
            HoldId = new HoldId(HoldId),
            AccountId = new AccountId(AccountId),
            Amount = new HoldAmount(Amount),
            CurrencyCode = Enum.Parse<CurrencyCode>(CurrencyCode),
            IdempotencyKey = new IdempotencyKey(Guid.NewGuid()),
            Type = (HoldType)HoldTypeId,
            Status = (HoldStatus)HoldStatusId,
            Source = (HoldSource)HoldSourceId,
            SettledTransactionId = SettledTransactionId.HasValue
                ? new TransactionId(SettledTransactionId.Value)
                : null,
            ExpiresAt = ExpiresAt.HasValue
                ? new ExpiresAt(ExpiresAt.Value)
                : null,
            Description = !string.IsNullOrWhiteSpace(Description)
                ? new HoldDescription(Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(Reference)
                ? new HoldReference(Reference)
                : null,
            CreatedAt = new CreatedAt(CreatedAt),
            CreatedBy = new Username(CreatedBy),
            UpdatedAt = new UpdatedAt(UpdatedAt),
            UpdatedBy = new Username(UpdatedBy),
            IsDeleted = IsDeleted,
            DeletedAt = DeletedAt.HasValue
                ? new DeletedAt(DeletedAt.Value)
                : null,
            DeletedBy = !string.IsNullOrWhiteSpace(DeletedBy)
                ? new Username(DeletedBy)
                : null
        };
    }

    public AccountEntity AccountEntity { get; init; } = null!;
    public TransactionEntity? SettledTransactionEntity { get; init; }
    public HoldTypeEntity HoldTypeEntity { get; init; } = null!;
    public HoldStatusEntity HoldStatusEntity { get; init; } = null!;
    public HoldSourceEntity HoldSourceEntity { get; init; } = null!;
    public HistoryTypeEntity HistoryTypeEntity { get; init; } = null!;
    public ProcessingStatusEntity ProcessingStatusEntity { get; init; } = null!;
}
