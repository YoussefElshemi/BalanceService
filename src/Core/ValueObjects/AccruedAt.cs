using Core.Constants;

namespace Core.ValueObjects;

public readonly record struct AccruedAt
{
    public AccruedAt(DateTimeOffset value)
    {
        Value = value;
    }

    private DateTimeOffset Value { get; }

    private DateOnly ToDateOnly() => DateOnly.FromDateTime(Value.UtcDateTime);

    public static implicit operator DateTimeOffset(AccruedAt valueObject)
    {
        return valueObject.Value;
    }

    public static implicit operator DateOnly(AccruedAt valueObject)
    {
        return valueObject.ToDateOnly();
    }

    public override string ToString()
    {
        return Value.ToString(DateTimeConstants.DateTimeFormat);
    }
}