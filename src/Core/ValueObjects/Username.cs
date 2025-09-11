namespace Core.ValueObjects;

public readonly record struct Username
{
    public Username(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(Username valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}