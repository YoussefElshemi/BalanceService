using Core.Constants;

namespace Core.ValueObjects;

public readonly record struct ExpiresAt
{
    public ExpiresAt(DateTimeOffset value)
    {
        Value = value;
    }

    private DateTimeOffset Value { get; }

    public static implicit operator DateTimeOffset(ExpiresAt valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(DateTimeConstants.DateFormat);
    }
}