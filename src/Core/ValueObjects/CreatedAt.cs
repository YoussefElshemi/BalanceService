using Core.Constants;

namespace Core.ValueObjects;

public readonly record struct CreatedAt
{
    public CreatedAt(DateTimeOffset value)
    {
        Value = value;
    }

    private DateTimeOffset Value { get; }

    public static implicit operator DateTimeOffset(CreatedAt valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(DateTimeConstants.DateTimeFormat);
    }
}