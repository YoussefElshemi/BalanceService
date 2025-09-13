using Core.Constants;

namespace Core.ValueObjects;

public readonly record struct StatementDate
{
    public StatementDate(DateOnly value)
    {
        Value = value;
    }

    private DateOnly Value { get; }

    public static implicit operator DateOnly(StatementDate valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(DateTimeConstants.DateFormat);
    }
}