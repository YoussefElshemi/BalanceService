using Core.Constants;

namespace Core.ValueObjects;

public readonly record struct PostedDate
{
    public PostedDate(DateOnly value)
    {
        Value = value;
    }

    private DateOnly Value { get; }

    public static implicit operator DateOnly(PostedDate valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(DateTimeConstants.DateFormat);
    }
}