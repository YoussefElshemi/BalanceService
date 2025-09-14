using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Entities;
using Infrastructure.Entities.History;

namespace Infrastructure.Services;

public class HoldHistoryService(
    IHistoryRepository<HoldHistoryEntity, HoldHistory> repository)
    : HistoryService<HoldHistoryEntity, HoldHistory>(repository)
{
    protected override Dictionary<string, string> FieldMappings { get; } = new()
    {
        { nameof(HoldEntity.HoldTypeId), nameof(Hold.Type) },
        { nameof(HoldEntity.HoldStatusId), nameof(Hold.Status) },
        { nameof(HoldEntity.HoldSourceId), nameof(Hold.Source) }
    };

    protected override Dictionary<string, Func<string?, string?>> ValueMappers { get; } = new()
    {
        { nameof(HoldEntity.HoldTypeId), v => Enum.TryParse<HoldType>(v, out var type) ? type.ToString() : v },
        { nameof(HoldEntity.HoldStatusId), v => Enum.TryParse<HoldStatus>(v, out var status) ? status.ToString() : v },
        { nameof(HoldEntity.HoldSourceId), v => Enum.TryParse<HoldSource>(v, out var source) ? source.ToString() : v }
    };
}