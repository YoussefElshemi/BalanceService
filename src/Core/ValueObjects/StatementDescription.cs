namespace Core.ValueObjects;

public readonly record struct StatementDescription
{
    public StatementDescription(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(StatementDescription valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}