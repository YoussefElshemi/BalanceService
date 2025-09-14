using Core.Models;
using Presentation.Models;

namespace Presentation.Mappers;

public static class ChangeEventMapper
{
    public static ChangeEventDto ToDto(this ChangeEvent changeEvent)
    {
        return new ChangeEventDto
        {
            EntityId = changeEvent.EntityId,
            Timestamp = changeEvent.Timestamp,
            Field = changeEvent.Field,
            OldValue = changeEvent.OldValue,
            NewValue = changeEvent.NewValue
        };
    }
}