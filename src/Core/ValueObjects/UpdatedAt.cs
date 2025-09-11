using Core.Constants;

namespace Core.ValueObjects;

public readonly record struct UpdatedAt
{
    public UpdatedAt(DateTimeOffset value)
    {
        Value = value;
    }

    private DateTimeOffset Value { get; }

    public static implicit operator DateTimeOffset(UpdatedAt valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(DateTimeConstants.DateTimeFormat);
    }
}