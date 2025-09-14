namespace Core.ValueObjects;

public readonly record struct ChangeEventValue
{
    public ChangeEventValue(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(ChangeEventValue valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}