using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;

namespace Infrastructure.Services;

public class HoldHistoryService(
    IHistoryRepository<HoldHistoryEntity, HoldHistory> repository)
    : HistoryService<HoldHistoryEntity, HoldHistory>(repository)
{
    protected override Dictionary<ChangeEventField, ChangeEventField> FieldMappings { get; } = new()
    {
        { new ChangeEventField(nameof(HoldEntity.HoldTypeId)), new ChangeEventField(nameof(Hold.Type)) },
        { new ChangeEventField(nameof(HoldEntity.HoldStatusId)), new ChangeEventField(nameof(Hold.Status)) },
        { new ChangeEventField(nameof(HoldEntity.HoldSourceId)), new ChangeEventField(nameof(Hold.Source)) }
    };

    protected override Dictionary<string, Func<ChangeEventValue?, ChangeEventValue?>> ValueMappers { get; } = new()
    {
        { nameof(HoldEntity.HoldTypeId), v => Enum.TryParse<HoldType>(v, out var value)
            ? new ChangeEventValue(value.ToString())
            : v
        },
        { nameof(HoldEntity.HoldStatusId), v => Enum.TryParse<HoldStatus>(v, out var value)
            ? new ChangeEventValue(value.ToString())
            : v
        },
        { nameof(HoldEntity.HoldSourceId), v => Enum.TryParse<HoldSource>(v, out var value)
            ? new ChangeEventValue(value.ToString())
            : v
        }
    };
}