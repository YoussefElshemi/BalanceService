namespace Core.ValueObjects;

public readonly record struct StatementReference
{
    public StatementReference(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(StatementReference valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}