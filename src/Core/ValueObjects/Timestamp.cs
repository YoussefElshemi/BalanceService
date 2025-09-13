using Core.Constants;

namespace Core.ValueObjects;

public readonly record struct Timestamp
{
    public Timestamp(DateTimeOffset value)
    {
        Value = value;
    }

    private DateTimeOffset Value { get; }

    public static implicit operator DateTimeOffset(Timestamp valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(DateTimeConstants.DateFormat);
    }
}