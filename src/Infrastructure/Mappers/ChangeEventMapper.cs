using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities.History;

namespace Infrastructure.Mappers;

public static class ChangeEventMapper
{
    public static ChangeEvent ToModel(this ChangeEventEntity changeEventEntity)
    {
        return new ChangeEvent
        {
            EntityId = new EntityId(changeEventEntity.EntityId),
            Timestamp = new Timestamp(changeEventEntity.Timestamp),
            Field = new ChangeEventField(changeEventEntity.Field),
            OldValue = changeEventEntity.OldValue != null
                ? new ChangeEventValue(changeEventEntity.OldValue)
                : null,
            NewValue = changeEventEntity.NewValue != null
                ? new ChangeEventValue(changeEventEntity.NewValue)
                : null,
        };
    }
}