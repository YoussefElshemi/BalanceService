using Core.Constants;

namespace Core.ValueObjects;

public readonly record struct ProcessedAt
{
    public ProcessedAt(DateTimeOffset value)
    {
        Value = value;
    }

    private DateTimeOffset Value { get; }

    public static implicit operator DateTimeOffset(ProcessedAt valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(DateTimeConstants.DateTimeFormat);
    }
}