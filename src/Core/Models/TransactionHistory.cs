using Core.Dtos;
using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record TransactionHistory : Transaction, IHistory<HistoryDto<TransactionHistoryDto>>
{
    public Guid GetPrimaryKey() => TransactionHistoryId;
    public required TransactionHistoryId TransactionHistoryId { get; init; }
    public required HistoryType HistoryType { get; init; }
    public required Timestamp Timestamp { get; init; }

    public HistoryDto<TransactionHistoryDto> ToDto()
    {
        return new HistoryDto<TransactionHistoryDto>
        {
            IdempotencyKey = TransactionHistoryId,
            Type = HistoryType,
            Timestamp = Timestamp,
            Data = new TransactionHistoryDto
            {
                TransactionId = TransactionId,
                AccountId = AccountId,
                Amount = Amount,
                CurrencyCode = CurrencyCode,
                Direction = Direction,
                PostedDate = PostedAt,
                IdempotencyKey = IdempotencyKey,
                Type = Type,
                Status = Status,
                Source = Source,
                Description = Description,
                Reference = Reference,
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