using Core.Dtos;
using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record HoldHistory : Hold, IHistory<HistoryDto<HoldHistoryDto>>
{
    public Guid GetPrimaryKey() => HoldHistoryId;
    public required HoldHistoryId HoldHistoryId { get; init; }
    public required HistoryType HistoryType { get; init; }
    public required Timestamp Timestamp { get; init; }

    public HistoryDto<HoldHistoryDto> ToDto()
    {
        return new HistoryDto<HoldHistoryDto>
        {
            IdempotencyKey = HoldHistoryId,
            Type = HistoryType,
            Timestamp = Timestamp,
            Data = new HoldHistoryDto
            {
                HoldId = HoldId,
                AccountId = AccountId,
                Amount = Amount,
                CurrencyCode = CurrencyCode,
                IdempotencyKey = IdempotencyKey,
                Type = Type,
                Status = Status,
                Source = Source,
                SettledTransactionId = SettledTransactionId,
                ExpiresAt = ExpiresAt,
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