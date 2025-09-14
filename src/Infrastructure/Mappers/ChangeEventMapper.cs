using Core.Models;
using Infrastructure.Entities.History;

namespace Infrastructure.Mappers;

public static class ChangeEventMapper
{
    public static ChangeEvent ToModel(this ChangeEventEntity changeEventEntity)
    {
        return new ChangeEvent
        {
            EntityId = changeEventEntity.EntityId,
            Timestamp = changeEventEntity.Timestamp,
            Field = changeEventEntity.Field,
            OldValue = changeEventEntity.OldValue,
            NewValue = changeEventEntity.NewValue
        };
    }
}