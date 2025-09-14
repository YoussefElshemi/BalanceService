using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;
using Infrastructure.Entities.History;

namespace Infrastructure.Services;

public class TransactionHistoryService(
    IHistoryRepository<TransactionHistoryEntity, TransactionHistory> repository)
    : HistoryService<TransactionHistoryEntity, TransactionHistory>(repository)
{
    protected override Dictionary<ChangeEventField, ChangeEventField> FieldMappings { get; } = new()
    {
        { new ChangeEventField(nameof(TransactionEntity.TransactionTypeId)), new ChangeEventField(nameof(Transaction.Type)) },
        { new ChangeEventField(nameof(TransactionEntity.TransactionStatusId)), new ChangeEventField(nameof(Transaction.Status)) },
        { new ChangeEventField(nameof(TransactionEntity.TransactionDirectionId)), new ChangeEventField(nameof(Transaction.Direction)) },
        { new ChangeEventField(nameof(TransactionEntity.TransactionSourceId)), new ChangeEventField(nameof(Transaction.Source)) }
    };

    
    protected override Dictionary<string, Func<ChangeEventValue?, ChangeEventValue?>> ValueMappers { get; } = new()
    {
        { nameof(TransactionEntity.TransactionTypeId), v => Enum.TryParse<TransactionType>(v, out var value)
            ? new ChangeEventValue(value.ToString())
            : v
        },
        { nameof(TransactionEntity.TransactionStatusId), v => Enum.TryParse<TransactionStatus>(v, out var value)
            ? new ChangeEventValue(value.ToString())
            : v
        },
        { nameof(TransactionEntity.TransactionDirectionId), v => Enum.TryParse<TransactionDirection>(v, out var value)
            ? new ChangeEventValue(value.ToString())
            : v
        },
        { nameof(TransactionEntity.TransactionSourceId), v => Enum.TryParse<TransactionSource>(v, out var value)
            ? new ChangeEventValue(value.ToString())
            : v
        }
    };
}