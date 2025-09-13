using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class RangeMapper
{
    public static RangeDto<T> ToDto<T>(this Range<T> range)
    {
        return new RangeDto<T>
        {
            From = range.From,
            To = range.To
        };
    }
}