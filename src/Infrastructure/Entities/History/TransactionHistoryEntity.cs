using Core.Enums;
using Core.Models;
using Core.ValueObjects;

namespace Infrastructure.Entities.History;

public record TransactionHistoryEntity : IHistoryEntity<TransactionHistory>
{
    public Guid GetPrimaryKey() => TransactionHistoryId;
    public required Guid TransactionHistoryId { get; init; }
    public required int HistoryTypeId { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required Guid TransactionId { get; init; }
    public required Guid AccountId { get; init; }
    public required decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required int TransactionDirectionId { get; init; }
    public required DateTimeOffset? PostedAt  { get; init; }
    public required Guid IdempotencyKey { get; init; }
    public required int TransactionTypeId { get; init; }
    public required int TransactionStatusId { get; init; }
    public required int TransactionSourceId { get; init; }
    public required string? Description { get; init; }
    public required string? Reference { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTimeOffset? DeletedAt { get; init; }
    public required string? DeletedBy { get; init; }

    public required bool IsProcessed { get; set; }
    public required DateTimeOffset? ProcessedAt { get; set; }

    public TransactionHistory ToModel()
    {
        return new TransactionHistory
        {
            TransactionHistoryId = new TransactionHistoryId(TransactionHistoryId),
            HistoryType = (HistoryType)HistoryTypeId,
            Timestamp = new Timestamp(Timestamp),
            TransactionId = new TransactionId(TransactionId),
            AccountId = new AccountId(AccountId),
            Amount = new TransactionAmount(Amount),
            CurrencyCode = Enum.Parse<CurrencyCode>(CurrencyCode),
            Direction = (TransactionDirection)TransactionDirectionId,
            PostedAt = PostedAt.HasValue
                ? new PostedAt(PostedAt.Value)
                : null,
            IdempotencyKey = new IdempotencyKey(IdempotencyKey),
            Type = (TransactionType)TransactionTypeId,
            Status = (TransactionStatus)TransactionStatusId,
            Source = (TransactionSource)TransactionSourceId,
            Description = !string.IsNullOrWhiteSpace(Description)
                ? new TransactionDescription(Description)
                : null,
            Reference = !string.IsNullOrWhiteSpace(Reference)
                ? new TransactionReference(Reference)
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
                : null,
            IsProcessed = IsProcessed,
            ProcessedAt = ProcessedAt.HasValue
                ? new ProcessedAt(ProcessedAt.Value)
                : null,
        };
    }

    public AccountEntity AccountEntity { get; init; } = null!;
    public TransactionDirectionEntity TransactionDirectionEntity { get; init; } = null!;
    public TransactionTypeEntity TransactionTypeEntity { get; init; } = null!;
    public TransactionStatusEntity TransactionStatusEntity { get; init; } = null!;
    public TransactionSourceEntity TransactionSourceEntity { get; init; } = null!;
    public HistoryTypeEntity HistoryTypeEntity { get; init; } = null!;

}
