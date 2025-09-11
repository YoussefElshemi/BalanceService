namespace Core.ValueObjects;

public readonly record struct HoldDescription
{
    public HoldDescription(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(HoldDescription valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}