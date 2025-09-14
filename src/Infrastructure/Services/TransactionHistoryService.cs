using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Entities;
using Infrastructure.Entities.History;

namespace Infrastructure.Services;

public class TransactionHistoryService(
    IHistoryRepository<TransactionHistoryEntity, TransactionHistory> repository)
    : HistoryService<TransactionHistoryEntity, TransactionHistory>(repository)
{
    protected override Dictionary<string, string> FieldMappings { get; } = new()
    {
        { nameof(TransactionEntity.TransactionTypeId), nameof(Transaction.Type) },
        { nameof(TransactionEntity.TransactionStatusId), nameof(Transaction.Status) },
        { nameof(TransactionEntity.TransactionDirectionId), nameof(Transaction.Direction) },
        { nameof(TransactionEntity.TransactionSourceId), nameof(Transaction.Source) }
    };

    protected override Dictionary<string, Func<string?, string?>> ValueMappers { get; } = new()
    {
        { nameof(TransactionEntity.TransactionTypeId), v => Enum.TryParse<TransactionType>(v, out var type) ? type.ToString() : v },
        { nameof(TransactionEntity.TransactionStatusId), v => Enum.TryParse<TransactionStatus>(v, out var status) ? status.ToString() : v },
        { nameof(TransactionEntity.TransactionDirectionId), v => Enum.TryParse<TransactionDirection>(v, out var direction) ? direction.ToString() : v },
        { nameof(TransactionEntity.TransactionSourceId), v => Enum.TryParse<TransactionSource>(v, out var source) ? source.ToString() : v }
    };
}